using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace NeptunLight.Droid
{
    public class CalendarColorPool
    {
        private static CalendarColorPool _instance;

        private static readonly Color[][] ColorPool =
        {
            new[] {Color.ParseColor("#42a5f5"), Color.ParseColor("#bbdefb"), Color.ParseColor("#82b1ff"), Color.ParseColor("#2196f3")}, // blue
            new[] {Color.ParseColor("#cddc39"), Color.ParseColor("#aeea00"), Color.ParseColor("#dce775"), Color.ParseColor("#c0ca33") }, // lime
            new[] {Color.ParseColor("#ffeb3b"), Color.ParseColor("#ffea00"), Color.ParseColor("#fff176"), Color.ParseColor("#fdd835") }, // yellow
            new[] {Color.ParseColor("#ff5252"), Color.ParseColor("#ff8a80"), Color.ParseColor("#e57373"), Color.ParseColor("#ef9a9a")}, // red
            new[] {Color.ParseColor("#ab47bc"), Color.ParseColor("#e040fb"), Color.ParseColor("#ce93d8"), Color.ParseColor("#9c27b0") }, // purple
            new[] {Color.ParseColor("#ffa726"), Color.ParseColor("#fb8c00"), Color.ParseColor("#ffcc80"), Color.ParseColor("#ffd180") }, // orange
            new[] {Color.ParseColor("#26a69a"), Color.ParseColor("#1de9b6"), Color.ParseColor("#80cbc4"), Color.ParseColor("#009688") }, // teal
            new[] {Color.ParseColor("#66bb6a"), Color.ParseColor("#00c853"), Color.ParseColor("#81c784"), Color.ParseColor("#a5d6a7") }, // green
            new[] {Color.ParseColor("#ec407a"), Color.ParseColor("#ff80ab"), Color.ParseColor("#f06292"), Color.ParseColor("#e91e63") }, // pink
            new[] {Color.ParseColor("#a1887f"), Color.ParseColor("#d7ccc8"), Color.ParseColor("#8d6e63"), Color.ParseColor("#795548") }, // brown
        };

        private readonly Dictionary<string, Dictionary<string, (int mainIndex, int accentIndex)>> _allocationTable;

        private int _colorPoolPointer = 0;

        private CalendarColorPool(Dictionary<string, Dictionary<string, (int mainIndex, int accentIndex)>> allocationTable)
        {
            _allocationTable = allocationTable;
            Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(async _ =>
            {
                if (_dirty)
                {
                    await SaveAsync();
                }
            });
        }

        private static string FileLocation => Path.Combine(Application.Context.FilesDir.Path, "colorPool.json");

        private static bool _dirty = false;

        private bool IsInUse(string subject, int accentIndex)
        {
            lock (_allocationTable)
            {
                return _allocationTable.ContainsKey(subject) && _allocationTable[subject].Select(kvp => kvp.Value).Any(tpl => tpl.accentIndex == accentIndex);
            }
        }

        public Color this[string subject, string group]
        {
            get
            {
                lock (_allocationTable)
                {
                    // if outter does not exist
                    if (!_allocationTable.ContainsKey(subject))
                        _allocationTable[subject] = new Dictionary<string, (int mainIndex, int accentIndex)>();


                    // if inner does not exist
                    if (!_allocationTable[subject].ContainsKey(group))
                    {
                        int mainIndex;
                        int accentIndex;
                        if (_allocationTable[subject].Any())
                        {
                            // there is another entry for this group, find a new accent
                            mainIndex = _allocationTable[subject].First().Value.mainIndex;
                            accentIndex = 0;
                            for (int i = 0; i < ColorPool[mainIndex].Length; i++)
                            {
                                if (!IsInUse(subject, i))
                                {
                                    accentIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // no entry for this group yet, find a main index
                            mainIndex = _colorPoolPointer % ColorPool.Length;
                            _colorPoolPointer++;
                            accentIndex = 0;
                        }

                        _allocationTable[subject][group] = (mainIndex, accentIndex);
                        //_dirty = true;
                    }
                    
                    (int finalMainIndex, int finalAccentIndex) = _allocationTable[subject][group];
                    return ColorPool[finalMainIndex][finalAccentIndex];
                }
            }
        }

        public static async Task<CalendarColorPool> LoadAsync()
        {
            if (_instance == null)
                if (File.Exists(FileLocation))
                    await Task.Run(() =>
                    {
                        string text = File.ReadAllText(FileLocation);
                        _instance = new CalendarColorPool(JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, (int mainIndex, int accentIndex)>>>(text));
                    });
                else _instance = new CalendarColorPool(new Dictionary<string, Dictionary<string, (int mainIndex, int accentIndex)>>());
            return _instance;
        }

        public async Task SaveAsync()
        {
            await Task.Run(() =>
            {
                lock (_allocationTable)
                {
                    File.WriteAllText(FileLocation, JsonConvert.SerializeObject(_allocationTable));
                    _dirty = false;
                }
            });
        }
    }
}