using System;

using static ForwardTestTask.Engines.ICE.ICEngineConfig;


namespace ForwardTestTask.Engines.ICE
{
    class ICEngine : Engine
    {
        #region Config
        public readonly float MomentOfInertia;

        public readonly TorqueToSpeedDependence Dependence;

        public readonly float TorqueHeatingCoefficient;
        public readonly float SpeedHeatingCoefficient;
        #endregion

        private float _crankshaftSpeed;

        public ICEngine(float temperatureEnvironment, ICEngineConfig config)
            : base(temperatureEnvironment, config.TemperatureOverheat, config.ThermalExchangeCoefficient)
        {
            MomentOfInertia = config.MomentOfInertia;

            Dependence = config.Dependence;

            TorqueHeatingCoefficient = config.TorqueHeatingCoefficient;
            SpeedHeatingCoefficient = config.SpeedHeatingCoefficient;
        }

        public override void StopEngine()
        {
            base.StopEngine();

            _crankshaftSpeed = 0;
        }

        protected override void OnEngineRunnign()
        {
            base.OnEngineRunnign();

            _crankshaftSpeed += GetTorque() / MomentOfInertia;
        }

        protected override float Heating()
        {
            float result = GetTorque() * TorqueHeatingCoefficient
                + (float)Math.Pow(_crankshaftSpeed, 2) * SpeedHeatingCoefficient;

            return result;
        }

        private float GetTorque()
        {
            int i = 1;

            for (; i < Dependence.Length - 1; i++)
                if (_crankshaftSpeed < Dependence.Speed[i])
                    break;

            float result = (Dependence.Torque[i] - Dependence.Torque[i - 1])
                * (_crankshaftSpeed - Dependence.Speed[i - 1]) / (Dependence.Speed[i] - Dependence.Speed[i - 1]) + Dependence.Torque[i - 1];

            result = Math.Max(result, 0);

            return result;
        }
    }
}