﻿using HabitTracker.Server.Repository;
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
        private readonly IHabitRepository _habitRepository;

        public HabitLogService(IHabitLogRepository habitLogRepository, ILogger<HabitLogService> logger, IHabitRepository habitRepository)
        {
            _habitLogRepository = habitLogRepository;
            _logger = logger;
            _habitRepository = habitRepository;
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

        public Tuple<Habit, HabitLog>? Add(PostHabitLog postHabitLog, int userId)
        {
            try
            {
                LogHabit unit = new LogHabit(_logger, _habitRepository, _habitLogRepository, postHabitLog, userId);

                return unit.Execute();
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
