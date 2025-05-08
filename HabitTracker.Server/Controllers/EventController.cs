using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.SSE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Channels;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("events/[controller]")]
    public class EventController: Controller
    {
        private readonly IEventService<HabitTrackerEvent> _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService<HabitTrackerEvent> eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        private int GetUserId()
        {
            if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
            {
                throw new UnauthorizedException("Could not retrieve user id from JWT");
            }

            return userId;
        }

        [Authorize]
        [HttpGet("stream")]
        public async Task Stream(CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            try
            {
                _logger.LogInformation("EventController - Stream - settings content type header");
                Response.Headers["Content-Type"] = "text/event-stream";

                _logger.LogInformation("EventController - Stream - attempting to create channel for user {UserId}", userId);
                bool createSuccess = _eventService.CreateChannelForUser(userId);

                if (createSuccess == false)
                {
                    _logger.LogWarning("EventController - Stream - failed to create channel for user {UserId}", userId);
                    Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await Response.WriteAsync("Could not create SSE channel.");
                    return;
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("EventController - waiting to read from event service channel");
                    ChannelReader<HabitTrackerEvent>? channelReader = _eventService.GetReaderForUser(userId);

                    if (channelReader == null)
                    {
                        _logger.LogWarning("EventController - Stream - failed to retrieve channel for user {UserId}", userId);
                        Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await Response.WriteAsync("Could not retrieve SSE channel.");
                        return;
                    }

                    var hasData = await channelReader.WaitToReadAsync(cancellationToken);
                    if (!hasData)
                    {
                        _logger.LogInformation("EventController - Stream - channel closed for user {UserId}", userId);
                        break;
                    }

                    while (channelReader.TryRead(out var habitTrackerEvent))
                    {
                        _logger.LogInformation("EventController - Stream - processing event");
                        var message = JsonSerializer.Serialize(habitTrackerEvent);
                        await Response.WriteAsync($"data: {message}\n\n");
                        await Response.Body.FlushAsync();
                    }
                }
                _logger.LogInformation("EventController - Stream - cancellation requested");
            } 
            catch (OperationCanceledException)
            {
                _logger.LogError("EventController - Stream - request cancelled for user {UserId}", userId);
            }
            finally
            {
                bool deleteSuccess = _eventService.DeleteChannelForUser(userId);

                if (deleteSuccess == false)
                {
                    _logger.LogWarning("EventController - Stream - failed to delete channel for user {UserId}", userId);
                }
            }
        }
    }
}
