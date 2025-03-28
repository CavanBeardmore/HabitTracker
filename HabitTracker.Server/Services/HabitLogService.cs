using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Exceptions;
using System.Text.Json;

namespace HabitTracker.Server.Services
{

    public class HabitLogService : IHabitLogService
    {
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly ILogger<HabitLogService> _logger;

        public HabitLogService(IHabitLogRepository habitLogRepository, ILogger<HabitLogService> logger)
        {
            _habitLogRepository = habitLogRepository;
            _logger = logger;
        }

        public HabitLog? GetById(int habitLogId, int userId)
        {
            try
            {
                _logger.LogInformation("HabitLogService - GetById - gettting habit log by id");
                HabitLog? habitLog = _habitLogRepository.GetById(habitLogId, userId);

                if (habitLog != null)
                {
                    _logger.LogInformation("HabitLogService - GetById - found habit log");
                    return habitLog;
                }

                throw new NotFoundException($"Could not find habit log of id {habitLogId} for user of id {userId}");
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when getting habit log of id - {habitLogId}");
            }
        }

        public IReadOnlyCollection<HabitLog?> GetAllByHabitId(int habitId, int userId, int pageNumber)
        {
            try
            {
                _logger.LogInformation("HabitLogService - GetAllByHabitId - getting all habit logs by habit id");
                IReadOnlyCollection<HabitLog> habitLogs = _habitLogRepository.GetAllByHabitId(habitId, userId, pageNumber);

                if (habitLogs.Count() != 0)
                {
                    _logger.LogInformation("HabitLogService - GetAllByHabitId - found habit logs");
                    return habitLogs;
                }

                throw new NotFoundException($"Could not find any habit logs for habit id - {habitId} - for user - {userId}");
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when getting habit logs from habit id - {habitId} - for user - {userId}");
            }
        }

        public HabitLog? Add(PostHabitLog postHabitLog, int userId)
        {
            try
            {
                var existingHabitLog = _habitLogRepository.GetByHabitIdAndStartDate(postHabitLog.Habit_id, userId, postHabitLog.Start_date);

                if (existingHabitLog != null)
                {
                    throw new ConflictException("Habit log already exists for this date");
                }

                var result = _habitLogRepository.Add(postHabitLog);

                if (result != null)
                {
                    return result;
                }

                throw new AppException($"Failed to add habit logs with data - {JsonSerializer.Serialize(postHabitLog)}");
            }
            catch (ConflictException ex)
            {
                throw new ConflictException(ex.Message);
            }
            catch (AppException ex)
            {
                throw new AppException(ex.Message);
            }
            catch
            {
                throw new AppException("An error occurred when adding habit log");
            }
        }

        public bool Update(PatchHabitLog habitLog)
        {
            try
            {
                _logger.LogInformation("HabitLogService - Update - updating habit log");
                return _habitLogRepository.Update(habitLog);
            }
            catch
            {
                throw new AppException($"An error occured when updating habitLog with id - {habitLog.Id}");
            }
        }

        public bool Delete(int habitLogId, int userId)
        {
            try
            {
                _logger.LogInformation("HabitLogService - Delete - deleting habit log");
                return _habitLogRepository.Delete(habitLogId, userId);
            }
            catch
            {
                throw new AppException($"An error occured when deleting habitLog with id - {habitLogId}");
            }
        }
    }
}
