using System;
using System.Collections.Generic;

namespace DesignPatterns.Homework
{
    #region Observer Pattern Interfaces

    public interface IWeatherStation
    {
        void RegisterObserver(IWeatherObserver observer);
        void RemoveObserver(IWeatherObserver observer);
        void NotifyObservers();

        float Temperature { get; }
        float Humidity { get; }
        float Pressure { get; }
    }

    public interface IWeatherObserver
    {
        void Update(float temperature, float humidity, float pressure);
    }

    #endregion

    #region Weather Station Implementation

    public class WeatherStation : IWeatherStation
    {
        private List<IWeatherObserver> _observers;
        private float _temperature;
        private float _humidity;
        private float _pressure;

        public WeatherStation()
        {
            _observers = new List<IWeatherObserver>();
        }

        public void RegisterObserver(IWeatherObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine($"[WeatherStation] Registered observer: {observer.GetType().Name}");
        }

        public void RemoveObserver(IWeatherObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine($"[WeatherStation] Removed observer: {observer.GetType().Name}");
        }

        public void NotifyObservers()
        {
            Console.WriteLine("[WeatherStation] Notifying observers...");
            foreach (var observer in _observers)
            {
                observer.Update(_temperature, _humidity, _pressure);
            }
        }

        public float Temperature => _temperature;
        public float Humidity => _humidity;
        public float Pressure => _pressure;

        public void SetMeasurements(float temperature, float humidity, float pressure)
        {
            Console.WriteLine("\n--- Weather Station: Weather measurements updated ---");
            _temperature = temperature;
            _humidity = humidity;
            _pressure = pressure;

            Console.WriteLine($"Temperature: {_temperature}°C");
            Console.WriteLine($"Humidity: {_humidity}%");
            Console.WriteLine($"Pressure: {_pressure} hPa");

            NotifyObservers();
        }
    }

    #endregion

    #region Observer Implementations

    public class CurrentConditionsDisplay : IWeatherObserver
    {
        private float _temperature;
        private float _humidity;
        private float _pressure;

        public CurrentConditionsDisplay(IWeatherStation station)
        {
            station.RegisterObserver(this);
        }

        public void Update(float temperature, float humidity, float pressure)
        {
            _temperature = temperature;
            _humidity = humidity;
            _pressure = pressure;
        }

        public void Display()
        {
            Console.WriteLine($"[CurrentConditions] Temp: {_temperature}°C, Humidity: {_humidity}%, Pressure: {_pressure} hPa");
        }
    }

    public class StatisticsDisplay : IWeatherObserver
    {
        private List<float> _temperatureHistory;

        public StatisticsDisplay(IWeatherStation station)
        {
            _temperatureHistory = new List<float>();
            station.RegisterObserver(this);
        }

        public void Update(float temperature, float humidity, float pressure)
        {
            _temperatureHistory.Add(temperature);
        }

        public void Display()
        {
            float min = float.MaxValue, max = float.MinValue, sum = 0;

            foreach (var temp in _temperatureHistory)
            {
                if (temp < min) min = temp;
                if (temp > max) max = temp;
                sum += temp;
            }

            float avg = _temperatureHistory.Count > 0 ? sum / _temperatureHistory.Count : 0;
            Console.WriteLine($"[Statistics] Min: {min}°C, Max: {max}°C, Avg: {avg:F2}°C");
        }
    }

    public class ForecastDisplay : IWeatherObserver
    {
        private float _lastPressure = 0f;
        private string _forecast = "No data";

        public ForecastDisplay(IWeatherStation station)
        {
            station.RegisterObserver(this);
        }

        public void Update(float temperature, float humidity, float pressure)
        {
            if (_lastPressure == 0)
            {
                _forecast = "Stable weather";
            }
            else if (pressure > _lastPressure)
            {
                _forecast = "Improving weather";
            }
            else if (pressure < _lastPressure)
            {
                _forecast = "Cooler, rainy weather";
            }
            else
            {
                _forecast = "Same weather";
            }

            _lastPressure = pressure;
        }

        public void Display()
        {
            Console.WriteLine($"[Forecast] {_forecast}");
        }
    }

    #endregion

    #region Application

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Observer Pattern Homework - Weather Station\n");

            try
            {
                WeatherStation weatherStation = new WeatherStation();

                Console.WriteLine("Creating display devices...");
                CurrentConditionsDisplay currentDisplay = new CurrentConditionsDisplay(weatherStation);
                StatisticsDisplay statisticsDisplay = new StatisticsDisplay(weatherStation);
                ForecastDisplay forecastDisplay = new ForecastDisplay(weatherStation);

                Console.WriteLine("\nSimulating weather changes...");

                // First update
                weatherStation.SetMeasurements(25.2f, 65.3f, 1013.1f);
                Console.WriteLine("\n--- Displaying Information ---");
                currentDisplay.Display();
                statisticsDisplay.Display();
                forecastDisplay.Display();

                // Second update
                weatherStation.SetMeasurements(28.5f, 70.2f, 1012.5f);
                Console.WriteLine("\n--- Displaying Updated Information ---");
                currentDisplay.Display();
                statisticsDisplay.Display();
                forecastDisplay.Display();

                // Third update
                weatherStation.SetMeasurements(22.1f, 90.7f, 1009.2f);
                Console.WriteLine("\n--- Displaying Final Information ---");
                currentDisplay.Display();
                statisticsDisplay.Display();
                forecastDisplay.Display();

                // Remove one observer
                Console.WriteLine("\nRemoving CurrentConditionsDisplay...");
                weatherStation.RemoveObserver(currentDisplay);

                // Final update
                weatherStation.SetMeasurements(24.5f, 80.1f, 1010.3f);
                Console.WriteLine("\n--- Displaying Information After Removal ---");
                statisticsDisplay.Display();
                forecastDisplay.Display();

                Console.WriteLine("\nObserver Pattern demonstration complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    #endregion
}
