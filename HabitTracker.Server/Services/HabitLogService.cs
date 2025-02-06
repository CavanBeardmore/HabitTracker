using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.UnitsOfWork.HabitLogUnits;
using HabitTracker.Server.UnitsOfWork;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services.Responses;
using HabitTracker.Server.Services.Responses.HabitLogResponses;

namespace HabitTracker.Server.Services
{

    public class HabitLogService : IHabitLogService
    {
        private readonly IHabitLogRepository _habitLogRepository;

        public HabitLogService(IHabitLogRepository habitLogRepository)
        {
            _habitLogRepository = habitLogRepository;
        }

        public IServiceResponseWithData<HabitLog?> GetById(int habitLogId, int userId)
        {
            try
            {
                HabitLog? habitLog = _habitLogRepository.GetById(habitLogId, userId);

                if (habitLog != null)
                {
                    return new GetHabitLogByIdResponse(true, habitLog, null);
                }

                return new GetHabitLogByIdResponse(false, null, null);
            }
            catch (Exception ex)
            {

                return new GetHabitLogByIdResponse(false, null, ex.Message);
            }
        }

        public IServiceResponseWithData<IReadOnlyCollection<HabitLog?>> GetAllByHabitId(int habitId, int userId, int pageNumber)
        {
            try
            {
                IReadOnlyCollection<HabitLog> habitLogs = _habitLogRepository.GetAllByHabitId(habitId, userId, pageNumber);

                if (habitLogs.Count() != 0)
                {
                    return new GetAllHabitLogsByIdResponse(true, habitLogs, null);
                }

                return new GetAllHabitLogsByIdResponse(false, null, null);
            }
            catch (Exception ex)
            {
                return new GetAllHabitLogsByIdResponse(false, null, ex.Message);
            }
        }

        public IServiceResponse Add(PostHabitLog postHabitLog)
        {
            try
            {

                var addHabitWithMissingLogs = new AddHabitLogWithMissingLogs(postHabitLog, _habitLogRepository);

                UnitOfWorkResult<bool> addResult = addHabitWithMissingLogs.execute();

                if (addResult.Success)
                {
                    return new HabitLogsResponse(true, null);
                }

                UnitOfWorkResult<bool?> rollbackResult = addHabitWithMissingLogs.rollback();

                if (rollbackResult.Success == false)
                {
                    return new HabitLogsResponse(false, "Rollback unsuccessful");
                }

                return new HabitLogsResponse(false, "Rollback was successful");
            }
            catch (Exception ex)
            {
                return new HabitLogsResponse(false, ex.Message);
            }
        }

        public IServiceResponse Update(PatchHabitLog habitLog)
        {
            try
            {
                return new HabitLogsResponse(_habitLogRepository.Update(habitLog), null);
            }
            catch (Exception ex)
            {
                return new HabitLogsResponse(false, ex.Message);
            }
        }

        public IServiceResponse Delete(int habitLogId, int userId)
        {
            try
            {
                return new HabitLogsResponse(_habitLogRepository.Delete(habitLogId, userId), null);
            }
            catch (Exception ex)
            {
                return new HabitLogsResponse(false, ex.Message);
            }
        }
    }
}
