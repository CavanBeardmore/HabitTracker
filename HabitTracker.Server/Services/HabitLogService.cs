using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.UnitsOfWork;

namespace HabitTracker.Server.Services
{

    public class HabitLogService : IHabitLogService
    {
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly ILogger<HabitLogService> _logger;
        private readonly ITransactionalUnit<AddedHabitLogResult?, AddHabitLogData> _addHabitUnit;

        public HabitLogService(
            IHabitLogRepository habitLogRepository, 
            ILogger<HabitLogService> logger,
            ITransactionalUnit<AddedHabitLogResult?, AddHabitLogData> addHabitUnit
        )
        {
            _habitLogRepository = habitLogRepository;
            _logger = logger;
            _addHabitUnit = addHabitUnit;
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

        public HabitLog? GetMostRecentByHabitId(int habitId, int userId)
        {
            try
            {
                _logger.LogInformation("HabitLogService - GetMostRecentByHabitId - getting most recent habit log by habit id and user id");
                HabitLog? habitLog = _habitLogRepository.GetMostRecentHabitLog(habitId, userId);

                if (habitLog != null)
                {
                    _logger.LogInformation("HabitLogService - GetMostRecentByHabitId - found habit log");
                    return habitLog;
                }

                throw new NotFoundException($"No habit log found when getting most recent log using {habitId} and {userId}");
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch
            {
                throw new AppException($"An error occured when getting most recent habit log using {habitId} and {userId}");
            }
        }

        public Tuple<IReadOnlyCollection<HabitLog>, bool>? GetAllByHabitId(int habitId, int userId, uint pageNumber)
        {
            try
            {
                _logger.LogInformation("HabitLogService - GetAllByHabitId - getting all habit logs by habit id");

                Tuple<IReadOnlyCollection<HabitLog>, bool> result = _habitLogRepository.GetAllByHabitId(habitId, userId, pageNumber);

                if (result.Item1.Count() != 0)
                {
                    _logger.LogInformation("HabitLogService - GetAllByHabitId - found habit logs");
                    return result;
                }

                throw new NotFoundException($"Could not find any habit logs for habit id - {habitId} - for user - {userId}");
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new AppException($"An error occured when getting habit logs from habit id - {habitId} - for user - {userId} - {ex.Message}");
            }
        }

        public AddedHabitLogResult? Add(PostHabitLog postHabitLog, int userId)
        {
            try
            {
                return _addHabitUnit.Execute(new AddHabitLogData(postHabitLog, userId));
            }
            catch (ConflictException ex)
            {
                throw new ConflictException(ex.Message);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
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

        public HabitLog? Update(PatchHabitLog habitLog)
        {
            try
            {
                _logger.LogInformation("HabitLogService - Update - updating habit log");
                HabitLog? updatedLog = _habitLogRepository.Update(habitLog);

                if (updatedLog == null) 
                {
                    throw new AppException("An error occurred when updating habit log.");
                }

                return updatedLog;
            }
            catch (AppException ex)
            {
                throw new AppException(ex.Message);
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
