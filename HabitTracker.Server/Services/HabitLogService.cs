using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.UnitsOfWork.HabitLogUnits;
using HabitTracker.Server.UnitsOfWork;
using HabitTracker.Server.Models;
using HabitTracker.Server.Exceptions;
using System.Text.Json;
using HabitTracker.Server.Database.Entities;

namespace HabitTracker.Server.Services
{

    public class HabitLogService : IHabitLogService
    {
        private readonly IHabitLogRepository _habitLogRepository;

        public HabitLogService(IHabitLogRepository habitLogRepository)
        {
            _habitLogRepository = habitLogRepository;
        }

        public HabitLog? GetById(int habitLogId, int userId)
        {
            try
            {
                HabitLog? habitLog = _habitLogRepository.GetById(habitLogId, userId);

                if (habitLog != null)
                {
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
                IReadOnlyCollection<HabitLog> habitLogs = _habitLogRepository.GetAllByHabitId(habitId, userId, pageNumber);

                if (habitLogs.Count() != 0)
                {
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

        public IReadOnlyCollection<HabitLog?> Add(PostHabitLog postHabitLog)
        {
            try
            {

                var addHabitWithMissingLogs = new AddHabitLogWithMissingLogs(postHabitLog, _habitLogRepository);

                UnitOfWorkResult<IReadOnlyCollection<HabitLog?>> addResult = addHabitWithMissingLogs.execute();

                if (addResult.Success)
                {
                    return addResult.Data;
                }

                UnitOfWorkResult<bool?> rollbackResult = addHabitWithMissingLogs.rollback();

                throw new AppException($"Failed to add habit logs with data - {JsonSerializer.Serialize(postHabitLog)}. Rollback was {(rollbackResult.Success ? "successful" : "unsuccessful")}.");
            }
            catch
            {
                throw new AppException($"An error occured when adding habit log with data {JsonSerializer.Serialize(postHabitLog)}");
            }
        }

        public bool Update(PatchHabitLog habitLog)
        {
            try
            {
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
                return _habitLogRepository.Delete(habitLogId, userId);
            }
            catch
            {
                throw new AppException($"An error occured when deleting habitLog with id - {habitLogId}");
            }
        }
    }
}
