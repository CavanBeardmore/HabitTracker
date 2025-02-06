using HabitTracker.Server.Repository;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Database.Entities;
using HabitTracker.Server.Services.Responses;
using HabitTracker.Server.Services.Responses.HabitResponses;

namespace HabitTracker.Server.Services
{
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
