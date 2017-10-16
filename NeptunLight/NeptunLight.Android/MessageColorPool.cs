using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using Android.App;
using Android.Graphics;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace NeptunLight.Droid
{
    public class MessageColorPool
    {
        private static readonly Lazy<MessageColorPool> _instance = new Lazy<MessageColorPool>(() =>
        {
            if (File.Exists(FileLocation))
                return new MessageColorPool(JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(FileLocation)));
            return new MessageColorPool(new Dictionary<string, int>());
        });

        private static readonly Color[] _colorPool =
        {
            Color.ParseColor("#EF5350"),
            Color.ParseColor("#F44336"),
            Color.ParseColor("#E53935"),
            Color.ParseColor("#D32F2F"),
            Color.ParseColor("#C62828"),
            Color.ParseColor("#B71C1C"),
            Color.ParseColor("#EC407A"),
            Color.ParseColor("#E91E63"),
            Color.ParseColor("#D81B60"),
            Color.ParseColor("#C2185B"),
            Color.ParseColor("#AD1457"),
            Color.ParseColor("#880E4F"),
            Color.ParseColor("#BA68C8"),
            Color.ParseColor("#AB47BC"),
            Color.ParseColor("#9C27B0"),
            Color.ParseColor("#8E24AA"),
            Color.ParseColor("#7B1FA2"),
            Color.ParseColor("#6A1B9A"),
            Color.ParseColor("#4A148C"),
            Color.ParseColor("#9575CD"),
            Color.ParseColor("#7E57C2"),
            Color.ParseColor("#673AB7"),
            Color.ParseColor("#5E35B1"),
            Color.ParseColor("#512DA8"),
            Color.ParseColor("#4527A0"),
            Color.ParseColor("#311B92"),
            Color.ParseColor("#7986CB"),
            Color.ParseColor("#5C6BC0"),
            Color.ParseColor("#3F51B5"),
            Color.ParseColor("#3949AB"),
            Color.ParseColor("#303F9F"),
            Color.ParseColor("#283593"),
            Color.ParseColor("#1A237E"),
            Color.ParseColor("#1E88E5"),
            Color.ParseColor("#1976D2"),
            Color.ParseColor("#1565C0"),
            Color.ParseColor("#0D47A1"),
            Color.ParseColor("#039BE5"),
            Color.ParseColor("#0288D1"),
            Color.ParseColor("#0277BD"),
            Color.ParseColor("#01579B"),
            Color.ParseColor("#00ACC1"),
            Color.ParseColor("#0097A7"),
            Color.ParseColor("#00838F"),
            Color.ParseColor("#006064"),
            Color.ParseColor("#009688"),
            Color.ParseColor("#00897B"),
            Color.ParseColor("#00796B"),
            Color.ParseColor("#00695C"),
            Color.ParseColor("#43A047"),
            Color.ParseColor("#388E3C"),
            Color.ParseColor("#2E7D32"),
            Color.ParseColor("#1B5E20"),
            Color.ParseColor("#689F38"),
            Color.ParseColor("#558B2F"),
            Color.ParseColor("#33691E"),
            Color.ParseColor("#9E9D24"),
            Color.ParseColor("#827717"),
            Color.ParseColor("#F9A825"),
            Color.ParseColor("#F57F17"),
            Color.ParseColor("#FF8F00"),
            Color.ParseColor("#FF6F00"),
            Color.ParseColor("#EF6C00"),
            Color.ParseColor("#E65100"),
            Color.ParseColor("#F4511E"),
            Color.ParseColor("#E64A19"),
            Color.ParseColor("#D84315"),
            Color.ParseColor("#A1887F"),
            Color.ParseColor("#8D6E63"),
            Color.ParseColor("#795548"),
            Color.ParseColor("#78909C"),
            Color.ParseColor("#607D8B"),
            Color.ParseColor("#546E7A"),
            Color.ParseColor("#455A64")
        };

        private readonly Dictionary<string, int> _allocationTable;
        private bool _dirty = false;
        private readonly Random _random = new Random();

        public MessageColorPool(Dictionary<string, int> allocationTable)
        {
            _allocationTable = allocationTable;
            Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(_ =>
            {
                if (_dirty)
                {
                    File.WriteAllText(FileLocation, JsonConvert.SerializeObject(_allocationTable));
                }
            });
        }

        public static MessageColorPool Instance => _instance.Value;

        private static string FileLocation => Path.Combine(Application.Context.FilesDir.Path, "messageColorPool.json");

        public Color this[string sender]
        {
            get
            {
                lock (_allocationTable)
                {
                    if (!_allocationTable.ContainsKey(sender))
                    {
                        _allocationTable[sender] = _random.Next(0, _colorPool.Length);
                        _dirty = true;
                    }

                    return _colorPool[_allocationTable[sender]];
                }
            }
        }


    }
}