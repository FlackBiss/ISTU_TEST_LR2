    using ClassLibrary1.Interfaces;
    using ClassLibrary1.Models;
    using Moq;
    using Xunit;

    namespace TestProject2.UnitTests;

    public class ShipTests
    {
        private readonly Mock<IEngine> engineMock;
        private readonly Mock<ICargoSystem> cargoSystemMock;
        private readonly Mock<NavigationSystem> navigationSystemMock;
        private readonly Mock<OperationLog> operationLogMock;
        private readonly Ship ship;

        public ShipTests()
        {
            engineMock = new Mock<IEngine>();
            cargoSystemMock = new Mock<ICargoSystem>();
            navigationSystemMock = new Mock<NavigationSystem>();
            operationLogMock = new Mock<OperationLog>();
            ship = new Ship("SHIP001", engineMock.Object, cargoSystemMock.Object, navigationSystemMock.Object,
                operationLogMock.Object);
        }

        // --- PROPERTY TESTS ---

        // Проверка корректного возврата идентификатора судна
        [Fact]
        public void Identifier_ReturnsCorrectValue()
        {
            Assert.Equal("SHIP001", ship.Identifier);
        }

        // Проверка, что судно изначально находится в состоянии Останов (Stopped)
        [Fact]
        public void State_InitiallyStopped()
        {
            Assert.Equal(ShipState.Stopped, ship.State);
        }

        // --- ОСНОВНЫЕ ПЕРЕХОДЫ СОСТОЯНИЙ ---

        // 8. Останов → Ожидание
        // Проверяет переход из состояния Останов в Ожидание через Start().
        [Fact]
        public void Start_FromStopped_ChangesStateToWaiting()
        {
            ship.Start();
            Assert.Equal(ShipState.Waiting, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Once());
        }

        // 9. Ожидание → Останов
        // Проверяет переход из состояния Ожидание в Останов через Stop().
        [Fact]
        public void Stop_FromWaiting_ChangesStateToStopped()
        {
            ship.Start();
            ship.Stop();
            Assert.Equal(ShipState.Stopped, ship.State);
            engineMock.Verify(e => e.TurnOff(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship stopped."), Times.Once());
        }

        // 14. Ожидание → Движение → Останов
        // Проверяет переход из Ожидания в Движение через NavigateTo и далее в Останов через Stop.
        [Fact]
        public void Stop_FromMoving_ChangesStateToStopped()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            ship.Stop();
            Assert.Equal(ShipState.Stopped, ship.State);
            engineMock.Verify(e => e.TurnOff(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship stopped."), Times.Once());
        }

        // 10. Ожидание → Погрузка → Ожидание
        // Проверяет процесс погрузки грузов: из Ожидания вызывается LoadCargo, возвращается в Ожидание.
        [Fact]
        public void LoadCargo_FromWaiting_ChangesStateToLoadingThenWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Load(100.0), Times.Once());
            operationLogMock.Verify(l => l.Log("Loaded 100 tons of cargo."), Times.Once());
        }

        // 3. Ожидание → Разгрузка → Ожидание
        // Проверяет процесс разгрузки: из Ожидания вызывается UnloadCargo, возвращается в Ожидание.
        [Fact]
        public void UnloadCargo_FromWaiting_ChangesStateToUnloadingThenWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.UnloadCargo();
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Unload(), Times.Once());
            operationLogMock.Verify(l => l.Log("Cargo unloaded."), Times.Once());
        }

        // 11. Ожидание → Движение
        // Проверяет переход из Ожидания в Движение через NavigateTo.
        [Fact]
        public void NavigateTo_FromWaiting_ChangesStateToMoving()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            Assert.Equal(ShipState.Moving, ship.State);
            navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port B."), Times.Once());
        }

        // 12. Движение → Ожидание
        // Проверяет возвращение из Движения в Ожидание через Wait.
        [Fact]
        public void Wait_FromMoving_ChangesStateToWaiting()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(2)); // Start and Wait
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // 13. Останов → Ожидание (через Wait)
        // Проверяет переход в Ожидание из Останов через Wait.
        [Fact]
        public void Wait_FromStopped_ChangesStateToWaiting()
        {
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // --- СЛОЖНЫЕ СЦЕНАРИИ (СЕКВЕНЦИИ) ---

        // 1. Останов → Ожидание → Останов
        // Проверяет базовый цикл запуска и остановки.
        [Fact]
        public void Scenario_StoppedToWaitingToStopped()
        {
            ship.Start();
            ship.Stop();
            Assert.Equal(ShipState.Stopped, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            engineMock.Verify(e => e.TurnOff(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship stopped."), Times.Once());
        }

        // 2. Ожидание → Погрузка → Ожидание
        // Проверяет процесс погрузки грузов (секвенция).
        [Fact]
        public void Scenario_WaitingToLoadingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Load(100.0), Times.Once());
            operationLogMock.Verify(l => l.Log("Loaded 100 tons of cargo."), Times.Once());
        }

        // 3. Ожидание → Разгрузка → Ожидание
        // Проверяет процесс разгрузки грузов (секвенция).
        [Fact]
        public void Scenario_WaitingToUnloadingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.UnloadCargo();
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Unload(), Times.Once());
            operationLogMock.Verify(l => l.Log("Cargo unloaded."), Times.Once());
        }

        // 4. Ожидание → Движение → Ожидание
        // Проверяет процесс навигации и возврата в Ожидание.
        [Fact]
        public void Scenario_WaitingToMovingToWaiting()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(2)); // Start and Wait
            operationLogMock.Verify(l => l.Log("Navigating to Port B."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // 5. Ожидание → Погрузка → Ожидание → Движение → Ожидание
        // Проверяет комбинированный сценарий погрузки и последующей навигации.
        [Fact]
        public void Scenario_WaitingToLoadingToWaitingToMovingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            ship.NavigateTo("Port C");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Load(100.0), Times.Once());
            navigationSystemMock.Verify(n => n.SetDestination("Port C"), Times.Once());
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(2)); // Start and Wait
            operationLogMock.Verify(l => l.Log("Loaded 100 tons of cargo."), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port C."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // 6. Ожидание → Разгрузка → Ожидание → Движение → Ожидание
        // Проверяет комбинированный сценарий разгрузки и последующей навигации.
        [Fact]
        public void Scenario_WaitingToUnloadingToWaitingToMovingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.UnloadCargo();
            ship.NavigateTo("Port C");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Unload(), Times.Once());
            navigationSystemMock.Verify(n => n.SetDestination("Port C"), Times.Once());
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(2)); // Start and Wait
            operationLogMock.Verify(l => l.Log("Cargo unloaded."), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port C."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // 7. Ожидание → Погрузка → Ожидание → Разгрузка → Ожидание → Движение → Ожидание
        // Проверяет полный цикл операций: погрузка, разгрузка, навигация.
        [Fact]
        public void Scenario_WaitingToLoadingToWaitingToUnloadingToWaitingToMovingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            ship.UnloadCargo();
            ship.NavigateTo("Port C");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Load(100.0), Times.Once());
            cargoSystemMock.Verify(c => c.Unload(), Times.Once());
            navigationSystemMock.Verify(n => n.SetDestination("Port C"), Times.Once());
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(2)); // Start and Wait
            operationLogMock.Verify(l => l.Log("Loaded 100 tons of cargo."), Times.Once());
            operationLogMock.Verify(l => l.Log("Cargo unloaded."), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port C."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // --- ДОПОЛНИТЕЛЬНЫЕ/EDGE CASE СЕКВЕНЦИИ ---

        // Повторные загрузки: Ожидание → Погрузка → Ожидание → Погрузка → Ожидание
        [Fact]
        public void Scenario_WaitingToLoadingToWaitingToLoadingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            ship.LoadCargo(100.0);  
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Load(100.0), Times.Exactly(2));
            operationLogMock.Verify(l => l.Log("Loaded 100 tons of cargo."), Times.Exactly(2));
        }

        // Повторные разгрузки: Ожидание → Разгрузка → Ожидание → Разгрузка → Ожидание
        [Fact]
        public void Scenario_WaitingToUnloadingToWaitingToUnloadingToWaiting()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.UnloadCargo();
            ship.UnloadCargo();
            Assert.Equal(ShipState.Waiting, ship.State);
            cargoSystemMock.Verify(c => c.Unload(), Times.Exactly(2));
            operationLogMock.Verify(l => l.Log("Cargo unloaded."), Times.Exactly(2));
        }

        // Повторная навигация: Ожидание → Движение → Ожидание → Движение → Ожидание
        [Fact]
        public void Scenario_WaitingToMovingToWaitingToMovingToWaiting()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            ship.Wait();
            ship.NavigateTo("Port C");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
            navigationSystemMock.Verify(n => n.SetDestination("Port C"), Times.Once());
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(3)); // Start and two Wait calls
            operationLogMock.Verify(l => l.Log("Navigating to Port B."), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port C."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Exactly(2));
        }

        // Запуск, остановка, повторный запуск, движение, ожидание
        [Fact]
        public void Scenario_StoppedToWaitingToStoppedToWaitingToMovingToWaiting()
        {
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.Start();
            ship.Stop();
            ship.Start();
            ship.NavigateTo("Port B");
            ship.Wait();
            Assert.Equal(ShipState.Waiting, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Exactly(3)); // Two Start and one Wait
            engineMock.Verify(e => e.TurnOff(), Times.Once());
            navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Exactly(2));
            operationLogMock.Verify(l => l.Log("Ship stopped."), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port B."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Once());
        }

        // --- НЕВАЛИДНЫЕ ПЕРЕХОДЫ ---

        // Проверяет, что нельзя остановить судно в состоянии Останов
        [Fact]
        public void Stop_FromStopped_DoesNotChangeState()
        {
            var initialState = ship.State;
            ship.Stop();
            Assert.Equal(initialState, ship.State);
            engineMock.Verify(e => e.TurnOff(), Times.Never());
            operationLogMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never());
        }

        // Проверяет, что нельзя загрузить в состоянии Останов
        [Fact]
        public void LoadCargo_FromStopped_DoesNotChangeState()
        {
            var initialState = ship.State;
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            Assert.Equal(initialState, ship.State);
            cargoSystemMock.Verify(c => c.Load(It.IsAny<double>()), Times.Never());
            operationLogMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never());
        }

        // Проверяет, что нельзя разгрузить в состоянии Останов
        [Fact]
        public void UnloadCargo_FromStopped_DoesNotChangeState()
        {
            var initialState = ship.State;
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.UnloadCargo();
            Assert.Equal(initialState, ship.State);
            cargoSystemMock.Verify(c => c.Unload(), Times.Never());
            operationLogMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never());
        }

        // Проверяет, что нельзя начать навигацию в состоянии Останов
        [Fact]
        public void NavigateTo_FromStopped_DoesNotChangeState()
        {
            var initialState = ship.State;
            ship.NavigateTo("Port B");
            Assert.Equal(initialState, ship.State);
            navigationSystemMock.Verify(n => n.SetDestination(It.IsAny<string>()), Times.Never());
            operationLogMock.Verify(l => l.Log(It.IsAny<string>()), Times.Never());
        }

        // Проверяет, что нельзя повторно запустить судно в Ожидании
        [Fact]
        public void Start_FromWaiting_DoesNotChangeState()
        {
            ship.Start();
            var initialState = ship.State;
            ship.Start();
            Assert.Equal(initialState, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Once());
        }

        // Проверяет, что повторный вызов Wait в Ожидании не влияет на состояние
        [Fact]
        public void Wait_FromWaiting_DoesNotInvokeEngineOrLog()
        {
            ship.Start();
            var initialState = ship.State;
            ship.Wait();
            Assert.Equal(initialState, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship Waiting."), Times.Never());
        }

        // Проверяет, что нельзя запустить судно в Движении
        [Fact]
        public void Start_FromMoving_DoesNotChangeState()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            var initialState = ship.State;
            ship.Start();
            Assert.Equal(initialState, ship.State);
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Once());
        }

        // Проверяет, что нельзя загрузить груз в Движении
        [Fact]
        public void LoadCargo_FromMoving_DoesNotChangeState()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            var initialState = ship.State;
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.LoadCargo(100.0);
            Assert.Equal(initialState, ship.State);
            cargoSystemMock.Verify(c => c.Load(It.IsAny<double>()), Times.Never());
            operationLogMock.Verify(l => l.Log("Loaded 100 tons of cargo."), Times.Never());
        }

        // Проверяет, что нельзя разгрузить груз в Движении
        [Fact]
        public void UnloadCargo_FromMoving_DoesNotChangeState()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            var initialState = ship.State;
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.UnloadCargo();
            Assert.Equal(initialState, ship.State);
            cargoSystemMock.Verify(c => c.Unload(), Times.Never());
            operationLogMock.Verify(l => l.Log("Cargo unloaded."), Times.Never());
        }

        // Проверяет, что нельзя начать другую навигацию в Движении
        [Fact]
        public void NavigateTo_FromMoving_DoesNotChangeState()
        {
            ship.Start();
            ship.NavigateTo("Port B");
            var initialState = ship.State;
            ship.NavigateTo("Port C");
            Assert.Equal(initialState, ship.State);
            navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
            navigationSystemMock.Verify(n => n.SetDestination("Port C"), Times.Never());
            operationLogMock.Verify(l => l.Log("Navigating to Port C."), Times.Never());
        }

        // --- ПРОЧИЕ ТЕСТЫ ---

        // Проверка, что генерация отчёта не меняет состояние
        [Fact]
        public void GenerateReport_DoesNotChangeState()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(500.0);
            navigationSystemMock.Setup(n => n.CurrentDestination).Returns("Port B");
            var initialState = ship.State;
            var report = ship.GenerateReport();
            Assert.Equal(initialState, ship.State);
            Assert.Equal("Ship SHIP001: State=Waiting, Cargo=500 tons, Destination=Port B", report);
            operationLogMock.Verify(l => l.Log(It.IsAny<string>()), Times.Once()); // Only from Start
        }
        
        // 14. Ожидание → Движение → Останов (ещё раз, как сценарий)
        [Fact]
        public void Scenario_WaitingToMovingToStopped()
        {
            ship.Start();
            cargoSystemMock.Setup(c => c.CurrentWeight).Returns(100.0);
            ship.NavigateTo("Port B");
            ship.Stop();
            Assert.Equal(ShipState.Stopped, ship.State);
            navigationSystemMock.Verify(n => n.SetDestination("Port B"), Times.Once());
            engineMock.Verify(e => e.TurnOn(), Times.Once());
            engineMock.Verify(e => e.TurnOff(), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship started and moved to Waiting state."), Times.Once());
            operationLogMock.Verify(l => l.Log("Navigating to Port B."), Times.Once());
            operationLogMock.Verify(l => l.Log("Ship stopped."), Times.Once());
        }
    }
