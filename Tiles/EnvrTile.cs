using System;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Environment Tile", menuName = "Tiles/Environment Tile")]
    public class EnvrTile : TileBase
    {
        [SerializeField]
        public Sprite DefaultSprite;
        public GameObject DefaultGameObject;
        public float moveCost = 1.0f;
        public Tile.ColliderType DefaultColliderType = Tile.ColliderType.None;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = DefaultSprite;
            tileData.gameObject = DefaultGameObject;
            tileData.colliderType = DefaultColliderType;
            //base.GetTileData(position, tilemap, ref tileData);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnvrTile))]
    public class EnvrTileEditor : Editor
    {
        private EnvrTile tile { get { return (target as EnvrTile); } }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            tile.DefaultSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", tile.DefaultSprite, typeof(Sprite), false, null);
            tile.DefaultGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", tile.DefaultGameObject, typeof(GameObject), false, null);
            tile.DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", tile.DefaultColliderType);
            tile.moveCost = EditorGUILayout.FloatField("Movement cost", tile.moveCost);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);
        }

        // Shows the Tile sprite as the asset thumbnail
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (tile.DefaultSprite != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { tile.DefaultSprite, Color.white, width, height });
                        if (ret is Texture2D)
                            return ret as Texture2D;
                    }
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
        // Taken from RuleTileEditor.cs
        public static Type GetType(string TypeName)
        {
            var type = Type.GetType(TypeName);
            if (type != null)
                return type;

            if (TypeName.Contains("."))
            {
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }

            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }
    }
#endif
}