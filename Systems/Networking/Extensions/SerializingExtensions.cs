using LiteNetLib.Utils;
using UnityEngine;

namespace Core.Systems.Network.Extensions
{
    public struct TransformValue
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    public static class SerializingExtensions
    {
        public struct TransformValue2D
        {
            public Vector2 Position;
            public Vector2 Rotation;
            public Vector2 Scale;
        }

        public static void Put(this NetDataWriter writer, Vector3 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
            writer.Put(vector.z);
        }

        public static Vector3 GetVector3(this NetDataReader reader)
        {
            return new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        public static void Put(this NetDataWriter writer, Vector2 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2(reader.GetFloat(), reader.GetFloat());
        }

        public static TransformValue GetTransformValue(this NetDataReader reader)
        {
            return new TransformValue {Position = reader.GetVector3(), Rotation = reader.GetVector3(), Scale = reader.GetVector3()};
        }

        public static void Put(this NetDataWriter writer, TransformValue value)
        {
            writer.Put(value.Position);
            writer.Put(value.Rotation);
            writer.Put(value.Scale);
        }

        public static TransformValue GetTransformValue2D(this NetDataReader reader)
        {
            return new TransformValue {Position = reader.GetVector2(), Rotation = reader.GetVector2(), Scale = reader.GetVector2()};
        }

        public static void Put(this NetDataWriter writer, TransformValue2D value)
        {
            writer.Put(value.Position);
            writer.Put(value.Rotation);
            writer.Put(value.Scale);
        }
    }
}