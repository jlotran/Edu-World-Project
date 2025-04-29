using System;
using UnityEngine;
using Fusion;

namespace RGSK.Fusion
{
    public struct StatePayload : INetworkStruct
    {
        public int tick { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 angularVelocity { get; set; }

        public bool Equals(StatePayload other)
        {
            return tick == other.tick &&
                   position.Equals(other.position) &&
                   rotation.Equals(other.rotation) &&
                   velocity.Equals(other.velocity) &&
                   angularVelocity.Equals(other.angularVelocity);
        }

        public override string ToString()
        {
            return $"Tick: {tick}, Position: {position}, Rotation: {rotation.eulerAngles}, Velocity: {velocity}";
        }
    }

    public class CicularBuffer<T>
    {
        private T[] buffer;
        private int bufferSize;
        private int currentIndex;

        public CicularBuffer(int size)
        {
            this.bufferSize = size;
            this.buffer = new T[size];
            this.currentIndex = 0;
        }

        public void Add(T item, int index)
        {
            int bufferIndex = index % bufferSize;
            buffer[bufferIndex] = item;
            currentIndex = bufferIndex;
        }

        public T Get(int index)
        {
            return buffer[index % bufferSize];
        }

        public T GetLatest()
        {
            return buffer[currentIndex];
        }

        public void Clear()
        {
            buffer = new T[bufferSize];
            currentIndex = 0;
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < bufferSize;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }
    }

    public class NetworkTimer
    {
        private float timer;
        public float MinTimeBetweenTicks { get; }
        public int currentTick { get; private set; }

        public NetworkTimer (float serverTickRate)
        {
            MinTimeBetweenTicks = 1f / serverTickRate;
        }

        public void Update(float deltaTime)
        {
            timer += deltaTime;
        }

        public bool ShouldTick()
        {
            if (timer >= MinTimeBetweenTicks) 
            {
                timer -= MinTimeBetweenTicks;
                currentTick ++;
                return true;
            }

            return false;
        }
    }
}
