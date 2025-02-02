/*
 * Copyright (c) 2020 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tizen.Sensor
{
    /// <summary>
    /// The HeartRateMonitorBatch class registers callbacks of batch jobs for heart rate monitoring and provides batch data of the heart rate.
    /// </summary>
    /// <since_tizen> 8 </since_tizen>
    public sealed class HeartRateMonitorBatch : BatchSensor<HeartRateMonitorBatchData>
    {
        private static string HRMBatchKey = "http://tizen.org/feature/sensor.heart_rate_monitor.batch";

        private event EventHandler<SensorAccuracyChangedEventArgs> _accuracyChanged;

        /// <summary>
        /// List of converted HeartRateMonitorBatchData.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        protected override IReadOnlyList<HeartRateMonitorBatchData> ConvertBatchData()
        {
            List<HeartRateMonitorBatchData> list = new List<HeartRateMonitorBatchData>();
            Interop.SensorEventStruct sensorData;
            for (int i = 0; i < BatchedEvents.Count; i++)
            {
                sensorData = BatchedEvents[i];
                list.Add(new HeartRateMonitorBatchData(sensorData.timestamp, sensorData.accuracy, (HeartRateMonitorBatchState)sensorData.values[0], (int)sensorData.values[1], (int)sensorData.values[2]));
            }
            return list.AsReadOnly();
        }

        /// <summary>
        /// Get the accuracy of the HeartRateMonitorBatch data as enum <see cref="SensorDataAccuracy"/> type.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        /// <value> Accuracy, <seealso cref="SensorDataAccuracy"/>. </value>
        public SensorDataAccuracy Accuracy { get; private set; } = SensorDataAccuracy.Undefined;

        /// <summary>
        /// Return true or false based on whether the HeartRateMonitorBatch sensor is supported by the device.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        /// <value><c>true</c> if supported; otherwise <c>false</c>.</value>
        public static bool IsSupported
        {
            get
            {
                Log.Info(Globals.LogTag, "Checking if the HeartRateMonitorBatch is supported");
                return CheckIfSupported(SensorType.HRMBatch, HRMBatchKey);
            }
        }

        /// <summary>
        /// Return the number of HeartRateMonitorBatch sensors available on the system.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        /// <value> The count of HeartRateMonitorBatch sensors. </value>
        /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state.</exception>
        public static int Count
        {
            get
            {
                Log.Info(Globals.LogTag, "Getting the count of accelerometer sensors");
                return GetCount();
            }
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Tizen.Sensor.HeartRateMonitorBatch"/> class.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        /// <privilege>http://tizen.org/privilege/healthinfo</privilege>
        /// <privlevel>public</privlevel>
        /// <feature>http://tizen.org/feature/sensor.heart_rate_monitor.batch</feature>
        /// <exception cref="ArgumentException">Thrown when an invalid argument is used.</exception>
        /// <exception cref="NotSupportedException">Thrown when the sensor is not supported.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the application has no privilege to use the sensor.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state.</exception>
        /// <param name='index'>
        /// Index refers to a particular HeartRateMonitorBatch in case of multiple sensors.
        /// Default value is 0.
        /// </param>
        public HeartRateMonitorBatch(uint index = 0) : base(index)
        {
            Log.Info(Globals.LogTag, "Creating HeartRateMonitorBatch object");
        }

        internal override SensorType GetSensorType()
        {
            return SensorType.HRMBatch;
        }

        /// <summary>
        /// An event handler for storing the callback functions for the event corresponding to the change in the HeartRateMonitorBatch data.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        public event EventHandler<HeartRateMonitorBatchDataUpdatedEventArgs> DataUpdated;

        /// <summary>
        /// An event handler for accuracy changed events.
        /// If an event is added, a new accuracy change callback is registered for this sensor.
        /// If an event is removed, accuracy change callback is unregistered for this sensor.
        /// </summary>
        /// <since_tizen> 8 </since_tizen>
        public event EventHandler<SensorAccuracyChangedEventArgs> AccuracyChanged
        {
            add
            {
                if (_accuracyChanged == null)
                {
                    AccuracyListenStart();
                }
                _accuracyChanged += value;
            }
            remove
            {
                _accuracyChanged -= value;
                if (_accuracyChanged == null)
                {
                    AccuracyListenStop();
                }
            }
        }

        private static int GetCount()
        {
            IntPtr list;
            int count;
            int error = Interop.SensorManager.GetSensorList(SensorType.HRMBatch, out list, out count);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error getting sensor list for HeartRateMonitorBatch");
                count = 0;
            }
            else
                Interop.Libc.Free(list);
            return count;
        }

        /// <summary>
        /// Reads HeartRateMonitorBatch data synchronously.
        /// </summary>
        internal override void ReadData()
        {
            int error = Interop.SensorListener.ReadDataList(ListenerHandle, out IntPtr eventsPtr, out uint events_count);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error reading HeartRateMonitorBatch data");
                throw SensorErrorFactory.CheckAndThrowException(error, "Reading HeartRateMonitorBatch data failed");
            }
            UpdateBatchData(eventsPtr, events_count);
            Interop.SensorEventStruct sensorData = latestEvent();
            Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Accuracy = sensorData.accuracy;
            Interop.Libc.Free(eventsPtr);
        }

        private static Interop.SensorListener.SensorEventsCallback _callback;

        internal override void EventListenStart()
        {
            _callback = (IntPtr sensorHandle, IntPtr eventsPtr, uint events_count, IntPtr data) =>
            {
                UpdateBatchData(eventsPtr, events_count);
                Interop.SensorEventStruct sensorData = latestEvent();
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Accuracy = sensorData.accuracy;
                DataUpdated?.Invoke(this, new HeartRateMonitorBatchDataUpdatedEventArgs((IReadOnlyList<HeartRateMonitorBatchData>)Data));
            };
            int error = Interop.SensorListener.SetEventsCallback(ListenerHandle, _callback, IntPtr.Zero);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error setting event callback for HeartRateMonitorBatch sensor");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to set event callback for HeartRateMonitorBatch");
            }
        }

        internal override void EventListenStop()
        {
            int error = Interop.SensorListener.UnsetEventsCallback(ListenerHandle);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error unsetting event callback for HeartRateMonitorBatch sensor");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to unset event callback for HeartRateMonitorBatch");
            }
        }

        private static Interop.SensorListener.SensorAccuracyCallback _accuracyCallback;

        private void AccuracyListenStart()
        {
            _accuracyCallback = (IntPtr sensorHandle, ulong timestamp, SensorDataAccuracy accuracy, IntPtr data) =>
            {
                Timestamp = timestamp;
                Accuracy = accuracy;
                _accuracyChanged?.Invoke(this, new SensorAccuracyChangedEventArgs(timestamp, accuracy));
            };

            int error = Interop.SensorListener.SetAccuracyCallback(ListenerHandle, _accuracyCallback, IntPtr.Zero);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error setting accuracy event callback for HeartRateMonitorBatch sensor");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to set accuracy event callback for HeartRateMonitorBatch");
            }
        }

        private void AccuracyListenStop()
        {
            int error = Interop.SensorListener.UnsetAccuracyCallback(ListenerHandle);
            if (error != (int)SensorError.None)
            {
                Log.Error(Globals.LogTag, "Error unsetting accuracy event callback for HeartRateMonitorBatch sensor");
                throw SensorErrorFactory.CheckAndThrowException(error, "Unable to unset accuracy event callback for HeartRateMonitorBatch");
            }
        }
    }
}
