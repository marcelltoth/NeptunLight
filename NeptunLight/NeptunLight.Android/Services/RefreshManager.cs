using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AppCenter.Analytics;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;

namespace NeptunLight.Droid.Services
{
    public class RefreshManager
    {
        private readonly IDataStorage _dataStorage;
        private readonly INeptunInterface _client;

        public RefreshManager(IDataStorage dataStorage, INeptunInterface client)
        {
            _dataStorage = dataStorage;
            _client = client;
        }

        public async Task RefreshAsync()
        {
            if (!_client.HasCredentials())
                return;

            NeptunData loadedData = new NeptunData();
            loadedData.BasicData = await _client.RefreshBasicDataAsync();
            loadedData.SemesterInfo = await _client.RefreshSemestersAsnyc();
            loadedData.SubjectsPerSemester = await _client.RefreshSubjectsAsnyc();
            loadedData.ExamsPerSemester = await _client.RefreshExamsAsnyc();
            loadedData.Calendar = await _client.RefreshCalendarAsnyc();
            loadedData.Periods = await _client.RefreshPeriodsAsnyc();
            IList<Mail> messages = await _client.RefreshMessages().ToList();
            loadedData.Messages.Clear();
            loadedData.Messages.AddRange(messages);

            _dataStorage.CurrentData = loadedData;
            await _dataStorage.SaveDataAsync();
        }
    }
}