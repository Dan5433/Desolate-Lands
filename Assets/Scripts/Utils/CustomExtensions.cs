using System;
using System.IO;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace CustomExtensions
{
    public static class Extensions
    {
        public static TileBase[] GetAllTiles(this Tilemap tilemap)
        {
            return tilemap.GetTilesBlock(tilemap.cellBounds);
        }
        public static Vector3Int ToVector3Int(this Vector3 vector)
        {
            return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        }

        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public static Vector2 ExtendRaycast(this RaycastHit2D hit, float distance, Transform transform)
        {
            Ray2D ray = new(transform.position, transform.up);
            return ray.GetPoint(hit.distance + distance);
        }

        public static void ChangeColors(this ParticleSystem system, Color a, Color b)
        {
            var main = system.main;
            var color = main.startColor;
            color.colorMin = a;
            color.colorMax = b;
            main.startColor = color;
        }

        public static void Restart(this ParticleSystem system)
        {
            system.Stop();
            system.Play();
        }

        public static Quaternion AnglesToQuaternion(this Vector3 angles)
        {
            return new Quaternion() { eulerAngles = angles };
        }

        public static void PlayRandomClip(this AudioSource source, AudioClip[] audio)
        {
            if (source.isPlaying || audio.Length == 0) return;

            source.pitch = Random.Range(0.9f,1.1f);

            source.PlayOneShot(audio[Random.Range(0, audio.Length)]);
        }

        public static void WriteVector2Int(this BinaryWriter writer, Vector2Int v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
        }

        public static Vector2Int ReadVector2Int(this BinaryReader reader)
        {
            return new(reader.ReadInt32(), reader.ReadInt32());
        }
        
        public static ParticleSystem PlayOneShot(this ParticleSystem system, Vector3 position, Quaternion rotation)
        {
            var instance = UnityEngine.Object.Instantiate(system, position, rotation);
            instance.Play();
            UnityEngine.Object.Destroy(instance.gameObject, instance.main.duration);
            return instance;
        }
    }

}

[Serializable]
public enum Direction
{
    Up = 0,
    Down = 2,
    Left = 1,
    Right = 3,
}