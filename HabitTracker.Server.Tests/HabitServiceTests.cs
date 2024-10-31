using Moq;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Service;
using HabitTracker.Server.DTO;

namespace HabitTracker.Server.Tests
{

    public class HabitServiceTests
    {

        private readonly HabitService _service;
        private readonly Mock<IHabitRepository> _mockRepository;

        public HabitServiceTests()
        {
            _mockRepository = new Mock<IHabitRepository>();
            _service = new HabitService(_mockRepository.Object);
        }

        [Fact]
        public void GetAllByUsername_ReturnsListOfHabits()
        {
            string username = "Test";
            var expectedHabits = new List<Habit> { new Habit(1, "Test", "Test") };

            _mockRepository.Setup(repo => repo.GetAllByUsername(username)).Returns(expectedHabits);

            var result = _service.GetAllHabitsByUsername(username);

            Assert.NotNull(result);
            Assert.IsType<List<Habit>>(result);
            Assert.Contains(result, h => h.habit_id == expectedHabits[0].habit_id
                                      && h.username == expectedHabits[0].username
                                      && h.name == expectedHabits[0].name);
        }

        [Fact]
        public void GetById_ReturnsHabit()
        {
            int id = 12345;

            var expectedHabit = new Habit(1, "Test", "Test");

            _mockRepository.Setup(repo => repo.GetById(id)).Returns(expectedHabit);

            var result = _service.GetHabitByHabitId(id);

            Assert.NotNull(result);
            Assert.IsType<Habit>(result);
            Assert.True(expectedHabit.habit_id == result.habit_id);
            Assert.True(expectedHabit.username == result.username);
            Assert.True(expectedHabit.name == result.name);
        }

        [Fact]
        public void AddHabit_CallsRepositoryAddMethod()
        {
            Habit habit = new Habit(1, "Test", "Test");

            _service.AddHabit(habit);

            _mockRepository.Verify(r => r.Add(It.Is<Habit>(h => h.habit_id == habit.habit_id
                                                            && h.username == habit.username
                                                            && h.name == habit.name)), Times.Once);
        }

        [Fact]
        public void DeleteHabit_CallsRepositoryDeleteMethod()
        {
            int id = 12345;

            _service.DeleteHabit(id);

            _mockRepository.Verify(r => r.Delete(It.Is<int>(h => h == id)), Times.Once);
        }

        [Fact]
        public void UpdateHabit_CallsRepositoryUpdateMethod()
        {
            Habit habit = new Habit(1, "Test", "Test");

            _service.UpdateHabit(habit);

            _mockRepository.Verify(r => r.Update(It.Is<Habit>(h => h.habit_id == habit.habit_id
                                                            && h.username == habit.username
                                                            && h.name == habit.name)), Times.Once);
        }
    }
}