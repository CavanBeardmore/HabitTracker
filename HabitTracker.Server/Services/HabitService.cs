using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Database.Entities;

namespace HabitTracker.Server.Services
{
    public class GetHabitByIdResponse : IServiceResponseWithData<Habit?>
    {
        public bool Success { get; }
        public Habit? Data { get; }
        public string? Error { get; }

        public GetHabitByIdResponse(bool success, Habit? habitLog, string? error)
        {
            Success = success;
            Data = habitLog;
            Error = error;
        }
    }

    public class GetAllHabitByUserIdResponse : IServiceResponseWithData<IReadOnlyCollection<Habit>>
    {
        public bool Success { get; }
        public IReadOnlyCollection<Habit?> Data { get; }
        public string? Error { get; }

        public GetAllHabitByUserIdResponse(bool success, IReadOnlyCollection<Habit> habitLogs, string? error)
        {
            Success = success;
            Data = habitLogs;
            Error = error;
        }
    }

    public class HabitResponse : IServiceResponse
    {
        public bool Success { get; }
        public string? Error { get; }

        public HabitResponse(bool success, string? error)
        {
            Success = success;
            Error = error;
        }
    }

    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _habitRepository;

        public HabitService(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }

        public IServiceResponseWithData<Habit?> GetById(int habitId, int userId)
        {
            try
            {
                Habit? habit = _habitRepository.GetById(habitId, userId);

                if (habit == null)
                {
                    return new GetHabitByIdResponse(false, null, null);
                }

                return new GetHabitByIdResponse(true, habit, null);
            }
            catch (Exception ex)
            {
                return new GetHabitByIdResponse(false, null, ex.Message);
            }
        }

        public IServiceResponseWithData<IReadOnlyCollection<Habit>> GetAllByUserId(int userId)
        {
            try
            {
                IReadOnlyCollection<Habit?> habits = _habitRepository.GetAllByUserId(userId);
                Console.WriteLine("GOT HABITS BY USER ID", habits);

                if (habits.Count == 0)
                {
                    return new GetAllHabitByUserIdResponse(false, [], "0 records found");
                }

                return new GetAllHabitByUserIdResponse(true, habits, null);
            }
            catch (Exception ex)
            {
                return new GetAllHabitByUserIdResponse(false, null, ex.Message);
            }
        }

        public IServiceResponse Add(PostHabit habit)
        {
            try
            {
                return new HabitResponse(_habitRepository.Add(habit), null);
            }
            catch (Exception ex)
            {
                return new HabitResponse(false, ex.Message);
            }
        }

        public IServiceResponse Update(PatchHabit habit)
        {
            try
            {
                return new HabitResponse(_habitRepository.Update(habit), null);
            }
            catch (Exception ex)
            {
                return new HabitResponse(false, ex.Message);
            }
        }

        public IServiceResponse Delete(int habitId, int userId)
        {
            try
            {
                return new HabitResponse(_habitRepository.Delete(habitId, userId), null);
            } 
            catch (Exception ex) 
            {
                return new HabitResponse(false, ex.Message);
            }
        }
    }
}
