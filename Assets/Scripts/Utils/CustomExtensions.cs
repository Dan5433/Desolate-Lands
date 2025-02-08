using EditorAttributes;
using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
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
            int x = vector.x >= 0 ? (int)vector.x: (int)(vector.x - 1);
            int y = vector.y >= 0 ? (int)vector.y : (int)(vector.y - 1);
            int z = vector.z >= 0 ? (int)vector.z : (int)(vector.z - 1);
            return new Vector3Int(x,y,z); //vector is anchored to bottom-left corner.
        }

        public static Vector2Int ToVector2Int(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
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
    Left = 1,
    Down = 2,
    Right = 3,
}