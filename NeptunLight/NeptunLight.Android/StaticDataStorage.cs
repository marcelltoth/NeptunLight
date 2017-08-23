using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;

namespace NeptunLight.Droid
{
    public class InstanceDataStorage : IDataStorage
    {
        public NeptunData Data { get; set; }

        public Task<NeptunData> LoadDataAsync()
        {
            return Task.FromResult(Data);
        }

        public Task SaveDataAsync(NeptunData data)
        {
            Data = data;
            return Task.CompletedTask;
        }
    }
}