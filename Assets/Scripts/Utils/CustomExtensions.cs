using System;
using System.Collections;
using System.IO;
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
            if (audio.Length == 0) 
                return;

            source.pitch = Random.Range(0.9f,1.1f);

            source.PlayOneShot(audio[Random.Range(0, audio.Length)]);
        }

        public static void Write(this BinaryWriter writer, Vector2Int v)
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

        public static Coroutine RestartCoroutine(this MonoBehaviour mono, IEnumerator routine)
        {
            mono.StopCoroutine(routine);
            return mono.StartCoroutine(routine);
        }

        public static RaycastHit2D RaycastTo(this Vector3 self, Vector3 target, float distance, int layerMask, Color color)
        {
            var direction = target - self;
            #if UNITY_EDITOR
                        Debug.DrawRay(self, direction, color);
            #endif
            return Physics2D.Raycast(self, direction, distance, layerMask);
        }
        public static void DrawDiamond(this Vector3 center, float radius, Color color, float duration)
        {
            Vector3 top = new(center.x, center.y + radius);
            Vector3 bottom = new(center.x,center.y - radius);
            Vector3 left = new(center.x - radius, center.y);
            Vector3 right = new(center.x + radius, center.y);

            Debug.DrawLine(top, right, color, duration);
            Debug.DrawLine(right, bottom, color, duration);
            Debug.DrawLine(bottom, left, color, duration);
            Debug.DrawLine(left, top, color, duration);
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