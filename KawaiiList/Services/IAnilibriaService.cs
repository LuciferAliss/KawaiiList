﻿using KawaiiList.Models;

namespace KawaiiList.Services
{
    public interface IAnilibriaService
    {
        Task<List<AnilibriaTitle>> GetTitlesAsync(int count, CancellationToken token);
        Task<List<AnilibriaTitle>> SearchTitlesAsync(string query, CancellationToken token);
        Task<(List<AnilibriaTitle>, bool)> GetPageTitlesAsync(int page, CancellationToken token);
        Task<(List<AnilibriaTitle>, bool)> GetSortTitlesAsync(int page, string genre, int? year, CancellationToken token);
        Task<AnilibriaTitle> GetTitleIdAsync(int id, CancellationToken token);
        Task<List<ScheduleAnilibriaTitles>> GetScheduleAsync(CancellationToken token);
        Task<AnilibriaTitle> GetRandomAsync(CancellationToken token);
        Task<List<string>> GetGenresAsync(CancellationToken token);
        Task<List<int>> GetYearsAsync(CancellationToken token);
    }
}