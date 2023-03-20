using System;


namespace ForwardTestTask.Engines.ICE
{
    class ICEngineConfig
    {
        public readonly float MomentOfInertia;

        public readonly TorqueToSpeedDependence Dependence;

        public readonly float TemperatureOverheat;
        public readonly float TorqueHeatingCoefficient;
        public readonly float SpeedHeatingCoefficient;
        public readonly float ThermalExchangeCoefficient;

        public ICEngineConfig()
        {
            MomentOfInertia = 10;

            Dependence = new TorqueToSpeedDependence(
                new float[] { 20, 75 , 100 , 105, 75, 0 },
                new float[] { 0, 75 , 150 , 200, 250, 300 }
                );

            TemperatureOverheat = 110;

            TorqueHeatingCoefficient = 0.01f;
            SpeedHeatingCoefficient = 0.0001f;

            ThermalExchangeCoefficient = 0.1f;
        }

        public ICEngineConfig(float momentOfInertia, TorqueToSpeedDependence dependence,
            float temperatureOverheat, float torqueHeatingCoefficient,
            float speedHeatingCoefficient, float thermalExchangeCoefficient)
        {
            MomentOfInertia = momentOfInertia;

            Dependence = dependence;

            TemperatureOverheat = temperatureOverheat;

            TorqueHeatingCoefficient = torqueHeatingCoefficient;
            SpeedHeatingCoefficient = speedHeatingCoefficient;

            ThermalExchangeCoefficient = thermalExchangeCoefficient;
        }



        public struct TorqueToSpeedDependence
        {
            public float[] Torque;
            public float[] Speed;

            public int Length => Math.Min(Torque.Length, Speed.Length);

            public TorqueToSpeedDependence(float[] torque, float[] speed)
            {
                if (torque == null || speed == null)
                    throw new Exception("One or all sequences lengths is null");

                if (torque.Length != speed.Length)
                    throw new Exception("Sequences lengths are not equal");

                if (torque.Length < 2)
                    throw new Exception("Sequences must contain at least two elements");

                Torque = torque;
                Speed = speed;
            }
        }
    }
}