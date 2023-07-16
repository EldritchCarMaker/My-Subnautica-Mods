using FMOD;
using Oculus.Newtonsoft.Json.Utilities.LinqBridge;
using QModManager.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RootMotion.FinalIK.GenericPoser;
using Logger = QModManager.Utility.Logger;
using Random = UnityEngine.Random;

namespace LuckyBlock.MonoBehaviours
{
    public enum BlockType
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
        Precursor
    }

    internal class LuckyBlockRarity : MonoBehaviour
    {
        private Dictionary<BlockType, Color> _blockColors = new()
        {
            { BlockType.Common, Color.yellow },
            { BlockType.Uncommon, Color.green },
            { BlockType.Rare, new Color(0.75f, 0, 1) },
            { BlockType.Legendary, Color.red },
        };


        private static int _typeCount = -1;
        public static int TypeCount 
        { 
            get
            {
                if(_typeCount == -1 ) _typeCount = Enum.GetValues(typeof(BlockType)).Length;
                return _typeCount;
            } 
        }
        public BlockType Type { get; private set; }

        //simply here so that I don't accidentally set it. Harder to accidentally call a method
        public void SetBlockType(BlockType type) 
        { 
            Type = type;
            RefreshBlockFaces();
        }
        
        public void Awake()
        {
            Type = (BlockType)(Weighted(TypeCount) - 1);

            if (Main.DebugLogs) ErrorMessage.AddMessage($"Spawned {Type} lucky block");

            RefreshBlockFaces();
        }

        public void RefreshBlockFaces()
        {
            if (Type == BlockType.Precursor)
            {
                foreach (var face in GetComponentsInChildren<BlockFace>())
                    face.IsPrecursor = true;
                return;
            }

            var color = _blockColors[Type];
            foreach (var rend in GetComponentsInChildren<Renderer>()) rend.material.color = color;
        }

        /// <summary>
        /// Returns a random value between 1 and Max, where each number is half as likely as the previous. Such that 2 is half as likely as 1, 3 is half as likely as 2, etc. 
        /// </summary>
        /// <param name="Max">Upper bound of the random</param>
        /// <returns></returns>
        public static int Weighted(int Max)
        {
            var random = Random.Range(0, (1 << Max) - 1);
            var consumed = 0;
            for (int i = 1; i <= Max; i++)
            {
                var weight = 1 << (Max - i);
                consumed += weight;
                if (consumed > random)
                    return i;
            }

            return 0;
        }
    }
}