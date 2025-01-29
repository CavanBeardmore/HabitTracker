using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.UnitsOfWork.HabitLogUnits;
using HabitTracker.Server.UnitsOfWork;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Services
{

    public class GetHabitLogByIdResponse : IServiceResponseWithData<HabitLog?>
    {
        public bool Success { get; }
        public HabitLog? Data { get; }
        public string? Error { get; }

        public GetHabitLogByIdResponse(bool success, HabitLog? habitLog, string? error)
        {
            Success = success;
            Data = habitLog;
            Error = error;
        }
    }

    public class GetAllHabitLogsByIdResponse : IServiceResponseWithData<IReadOnlyCollection<HabitLog?>>
    {
        public bool Success { get; }
        public IReadOnlyCollection<HabitLog?> Data { get; }
        public string? Error { get; }

        public GetAllHabitLogsByIdResponse(bool success, IReadOnlyCollection<HabitLog?> habitLogs, string? error)
        {
            Success = success;
            Data = habitLogs;
            Error = error;
        }
    }

    public class HabitLogsResponse : IServiceResponse
    {
        public bool Success { get; }
        public string? Error { get; }

        public HabitLogsResponse(bool success, string? error)
        {
            Success = success;
            Error = error;
        }
    }

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
                var unitOfWork = new GetHabitLogBelongingToUser(habitLogId, userId, _habitLogRepository);

                UnitOfWorkResult<HabitLog?> result = unitOfWork.execute();

                if (result.Success)
                {
                    return new GetHabitLogByIdResponse(true, result.Data, null);
                }

                return new GetHabitLogByIdResponse(false, null, null);
            }
            catch (Exception ex)
            {

                return new GetHabitLogByIdResponse(false, null, ex.Message);
            }
        }

        public IServiceResponseWithData<IReadOnlyCollection<HabitLog?>> GetAllByHabitId(int habitLogId, int userId, int pageNumber)
        {
            try
            {
                var unitOfWork = new GetCollectionOfHabitLogsBelongingToUser(habitLogId, userId, pageNumber, _habitLogRepository);

                UnitOfWorkResult<IReadOnlyCollection<HabitLog>> result = unitOfWork.execute();

                if (result.Success)
                {
                    return new GetAllHabitLogsByIdResponse(true, result.Data, null);
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
