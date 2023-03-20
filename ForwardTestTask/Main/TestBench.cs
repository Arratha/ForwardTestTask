using System;

using ForwardTestTask.Engines;


namespace ForwardTestTask.Benches
{
    class TestBench
    {
        private Engine _engine;

        private int _secondsPassed;

        private OnTestEnd _onTestEnd;
        private OnTestError _onTestError;

        public delegate void OnTestEnd(float secondsPassed);
        public delegate void OnTestError();

        public void SetEngine(Engine engine)
        {
            _engine = engine;
        }

        #region OverheatTest
        public void StartOverheatTest(OnTestError noEngineCallback, OnTestEnd endCallback, OnTestError errorCallback)
        {
            if (_engine == null)
            {
                noEngineCallback.Invoke();
                return;
            }

            _onTestEnd = endCallback;
            _onTestError = errorCallback;

            _engine.OnTemperatureChange += CheckEngineTemperature;
            _engine.OnErrorCallback += AbortOverheatTest;

            _engine.RunEngine(() => _secondsPassed++);
        }

        private void CheckEngineTemperature(float temperature)
        {
            if (temperature > _engine.TemperatureOverheat)
                StopOverheatTest();
        }

        private void StopOverheatTest()
        {
            _engine.OnTemperatureChange -= CheckEngineTemperature;
            _engine.OnErrorCallback -= AbortOverheatTest;
            _engine.StopEngine();

            _onTestEnd?.Invoke(_secondsPassed);

            _secondsPassed = 0;
        }

        private void AbortOverheatTest()
        {
            _engine.OnTemperatureChange -= CheckEngineTemperature;
            _engine.OnErrorCallback -= AbortOverheatTest;
            _engine.StopEngine();

            _onTestError?.Invoke();

            _secondsPassed = 0;
        }
        #endregion
    }
}