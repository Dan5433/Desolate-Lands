using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        public static Vector2 ExtendRaycast(this RaycastHit2D hit, float distance, UnityEngine.Transform transform)
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

        public static Quaternion AnglesToQuaternion(this Vector3 angles)
        {
            return new Quaternion() { eulerAngles = angles };
        }
    }

}