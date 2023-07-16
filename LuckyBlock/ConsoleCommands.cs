using LuckyBlock.BlockEvents;
using LuckyBlock.MonoBehaviours;
using SMLHelper.V2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock
{
    internal class ConsoleCommands
    {
        [ConsoleCommand("BlockCount")]
        public static void OnBlockCountCommand()
        {
            var count = new Dictionary<BlockType, float>();
            foreach (var block in LuckyBlockSpawner.blocks)
            {
                if (count.TryGetValue(block.Rarity, out var value))
                    count[block.Rarity] = value + 1;
                else
                    count.Add(block.Rarity, 1);
            }
            ErrorMessage.AddMessage($"Counted {LuckyBlockSpawner.blocks.Count} blocks");
            foreach (var pair in count)
            {
                ErrorMessage.AddMessage($"Found {pair.Value} {pair.Key} blocks ({(int)((pair.Value / LuckyBlockSpawner.blocks.Count) * 100)}%)");
            }
        }

        [ConsoleCommand("EventList")]
        public static void EventListCommand()
        {
            ErrorMessage.AddMessage($"Found {LuckyBlockMono.luckyBlockEvents.Length} events");
            int i = 1;
            foreach(var even in LuckyBlockMono.luckyBlockEvents)
            {
                var name = even.ToString().Split(".".ToCharArray()).Last();
                ErrorMessage.AddMessage($"{i}: {name}");
                i++;
            }
        }

        private static LuckyBlockMono _blockMono = null;

        [ConsoleCommand("BlockEvent")]//simply plays the event. Helpful for obvious reasons
        public static string EventCommand(string eventName)
        {
            var chosenEvent = FindEventByName(eventName, SearchType.Both, out string name);
            if (chosenEvent == null) return $"Could not find event by name {eventName}";

            RefreshBlockMonoObject();
            chosenEvent.TriggerEffect(_blockMono);

            return $"Triggering event {name}";
        }

        [ConsoleCommand("TryBlockEvent")]//tries to play a specific event, helpful to see if the event conditions are working properly
        public static string TestEventCommand(string eventName, string blocktype = "Precursor")
        {
            var chosenEvent = FindEventByName(eventName, SearchType.Both, out string name);
            if (chosenEvent == null) return $"Could not find event by name {eventName}";

            RefreshBlockMonoObject();

            if (!Enum.TryParse<BlockType>(blocktype, true, out var type)) return $"Invalid block type {blocktype}";
            _blockMono.GetComponent<LuckyBlockRarity>().SetBlockType(type);

            if (!chosenEvent.CanTrigger(_blockMono)) return $"event {name} could not trigger with type {blocktype}!";

            chosenEvent.TriggerEffect(_blockMono);

            return $"Triggering event {name}";
        }
        
        [ConsoleCommand("GetBlockEvent")]//picks an event, helpful for just seeing what event you may have gotten
        public static string GetEventCommand(string blocktype = "Precursor", bool playEvent = false)
        {
            RefreshBlockMonoObject();

            if (!Enum.TryParse<BlockType>(blocktype, true, out var type)) return $"Invalid block type {blocktype}";
            _blockMono.GetComponent<LuckyBlockRarity>().SetBlockType(type);

            var chosenEvent = _blockMono.PickEvent();
            var name = chosenEvent.GetName();

            if (!chosenEvent.CanTrigger(_blockMono)) return $"event {name} could not trigger with type {blocktype}!";

            if(playEvent)
                chosenEvent.TriggerEffect(_blockMono);

            return playEvent ? $"Triggering event {name}" : $"Returned event {name}";
        }

        public static void RefreshBlockMonoObject()
        {
            if(!_blockMono)
            {
                var obj = GameObject.Instantiate(LuckyBlockSpawner.blockPrefab);
                _blockMono = obj.GetComponent<LuckyBlockMono>();
                obj.SetActive(false);
            }
            _blockMono.transform.position = Player.main.transform.position + (2 * MainCamera.camera.transform.forward);
        }

        public static LuckyBlockEvent FindEventByName(string eventName, SearchType searchType, out string eventNameFound)
        {
            if(searchType == SearchType.Both)
            {
                //try to find a direct match first
                LuckyBlockEvent chosenEvent = FindEventByName(eventName, SearchType.Equals, out eventNameFound);

                if (chosenEvent == null)//look for ones that contain the string, if a direct match couldn't be found
                    chosenEvent = FindEventByName(eventName, SearchType.Contains, out eventNameFound);

                return chosenEvent;
            }

            foreach (var @event in LuckyBlockMono.luckyBlockEvents)
            {
                var name = @event.GetName().ToLower();

                eventNameFound = name;

                if (searchType == SearchType.Equals && name == eventName)
                    return @event;

                else if (searchType == SearchType.Contains && name.Contains(eventName)) 
                    return @event;
            }

            eventNameFound = "";
            return null;
        }

        public enum SearchType
        {
            Contains,
            Equals,
            Both
        }
    }
}
