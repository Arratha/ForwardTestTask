using System;


namespace ForwardTestTask.Engines
{
    public delegate void OnEngineRunning();
    public delegate void OnTestError();

    interface IEngine
    {
        void RunEngine(OnEngineRunning runningCallback);
        void StopEngine();
    }

    abstract class Engine : IEngine
    {
        private EngineState _engineState = EngineState.Stopped;
        public enum EngineState { Running, StopRequested, Stopped }

        #region EngineConfig
        public readonly float TemperatureOverheat;
        protected readonly float TemperatureAmbient;

        private readonly float ThermalExchangeCoefficient;
        #endregion

        protected float _temperatureCurrent;

        public event Action OnErrorCallback;
        public event Action<float> OnTemperatureChange;

        public Engine(float temperatureEnvironment, float temperatureOverheat, float thermalExchangeCoefficient)
        {
            TemperatureOverheat = temperatureOverheat;
            TemperatureAmbient = temperatureEnvironment;

            _temperatureCurrent = temperatureEnvironment;

            ThermalExchangeCoefficient = thermalExchangeCoefficient;
        }

        public void RunEngine(OnEngineRunning callback)
        {
            _engineState = EngineState.Running;

            while (_engineState != EngineState.StopRequested)
            {
                CheckEngineCondition();
                callback.Invoke();
                OnEngineRunnign();
            }

            _engineState = EngineState.Stopped;
        }

        public virtual void StopEngine()
        {
            if (_engineState == EngineState.Running)
                _engineState = EngineState.StopRequested;
        }

        protected virtual void OnEngineRunnign()
        {
            GetNewTemperature();
        }

        private void CheckEngineCondition()
        {
            OnTemperatureChange?.Invoke(_temperatureCurrent);
        }

        #region TemperatureChanges
        private void GetNewTemperature()
        {
            float heating = Heating();
            float cooling = Cooling();

            if (heating + cooling < 0.0001f)
            {
                OnErrorCallback?.Invoke();
                return;
            }

            _temperatureCurrent += heating + cooling;
        }

        protected abstract float Heating();

        private float Cooling()
        {
            float temperatureChange = (TemperatureAmbient - _temperatureCurrent)
                * ThermalExchangeCoefficient;

            return temperatureChange;
        }
        #endregion
    }
}