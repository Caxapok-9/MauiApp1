using MauiApp1;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    public Dictionary<string, int> EventsCategories;

    public Dictionary<string, int> SanctionsCategories;

    public List<Player> RosterHome;

    public List<Player> RosterGuest;

    public async Task ClearAsync()
    {
        await _db.DeleteAllAsync<Event>();
        await _db.DeleteAllAsync<Set>();
        await _db.DeleteAllAsync<Player>();
        await _db.DeleteAllAsync<LineUpBegin>();
        await _db.DeleteAllAsync<Team>();
        await _db.DeleteAllAsync<MainInformation>();
        await _db.DeleteAllAsync<EventCategory>();
        await _db.DeleteAllAsync<Sanction>();
        await _db.DeleteAllAsync<SanctionCategory>();
    }

    public async Task InizializeAllTablesAsync()
    {
        await _db.CreateTableAsync<MainInformation>();
        await _db.CreateTableAsync<Event>();
        await _db.CreateTableAsync<Set>();
        await _db.CreateTableAsync<Player>();
        await _db.CreateTableAsync<LineUpBegin>();
        await _db.CreateTableAsync<Team>();
        await InitializeEventCategoryAsync();
        await _db.CreateTableAsync<Sanction>();
        await InitializeSanctionCategoryAsync();
    }

    public List<Player> GetRoster(Team team)
    {
        if (team.IsHome)
            return RosterHome;
        else
            return RosterGuest;
    }

    #region MainInfo

    public async Task InitializeMainInfoAsync() => await _db.CreateTableAsync<MainInformation>();

    public async Task<MainInformation> GetMainInfoAsync() => await _db.Table<MainInformation>().FirstOrDefaultAsync();

    public async Task<int> SaveMainInfoAsync(MainInformation info) => await _db.InsertAsync(info);

    public async Task<int> UpdateMainInfoAsync(MainInformation Info) => await _db.UpdateAsync(Info);

    public async Task<int> DeleteMainInfoAsync() => await _db.DeleteAllAsync<MainInformation>();

    #endregion

    #region EventCategory

    public async Task InitializeEventCategoryAsync()
    {
        await _db.CreateTableAsync<EventCategory>();

        await _db.DeleteAllAsync<EventCategory>();

        var list = new List<EventCategory>
        {
            new EventCategory() {Name = "S"},
            new EventCategory() {Name = "T"},
            new EventCategory() {Name = "R"},
            new EventCategory() {Name = "RR"},
            new EventCategory() {Name = "WR"}
        };

        await _db.InsertAllAsync(list);

        var l = await _db.Table<EventCategory>().ToListAsync();

        EventsCategories = l.ToDictionary(x => x.Name, x => x.Id);
    }

    #endregion

    #region SanctionCategory

    public async Task InitializeSanctionCategoryAsync()
    {
        await _db.CreateTableAsync<SanctionCategory>();

        await _db.DeleteAllAsync<SanctionCategory>();

        var list = new List<SanctionCategory>
        {
            new SanctionCategory() {Name = "Предупреждение (Жёлтая карточка)"},
            new SanctionCategory() {Name = "Замечание (Красная карточка)"},
            new SanctionCategory() {Name = "Удаление (Две карточки в одной руке)"},
            new SanctionCategory() {Name = "Дисквалификация (Две карточки в двух руках)"}
        };

        await _db.InsertAllAsync(list);

        var l = await _db.Table<SanctionCategory>().ToListAsync();

        SanctionsCategories = l.ToDictionary(x => x.Name, x => x.Id);
    }

    #endregion

    #region Player

    public async Task InitializePlayerAsync() => await _db.CreateTableAsync<Player>();

    public async Task ClearReplaceID() => await _db.ExecuteAsync("UPDATE Player SET ReplaceID = 0", false);

    public async Task ClearRemove() => await _db.ExecuteAsync("UPDATE Player SET IsRemove = FALSE", false);

    public async Task SavePlayerAsync(Player player) => await _db.InsertAsync(player);

    public async Task UpdatePlayerAsync(Player player) => await _db.UpdateAsync(player);

    public async Task<List<Player>> GetPlayerAsync() => await _db.Table<Player>().ToListAsync();

    public async Task<List<Player>> GetPlayerAsync(Team team) => await _db.Table<Player>().Where(x => x.TeamID == team.Id).ToListAsync();

    public async Task<int> DeletePlayerAsync() => await _db.DeleteAllAsync<Player>();

    #endregion

    #region Set

    public async Task InitializeSetAsync() => await _db.CreateTableAsync<Set>();

    public async Task<int> SaveSetAsync(Set set) => await _db.InsertAsync(set);

    public async Task<int> DeleteSetAsync() => await _db.DeleteAllAsync<Set>();

    public async Task<int> UpdateSetAsync(Set set) => await _db.UpdateAsync(set);

    public async Task<List<Set>> GetSetAsync() => await _db.Table<Set>().ToListAsync();

    public async Task<Set> GetLastSetAsync() => await _db.Table<Set>().OrderByDescending(x => x.Id).FirstOrDefaultAsync();

    #endregion

    #region Team

    public async Task InitializeTeamAsync() => await _db.CreateTableAsync<Team>();

    public async Task<int> SaveTeamAsync(Team team) => await _db.InsertAsync(team);

    public async Task<int> DeleteTeamAsync() => await _db.DeleteAllAsync<Team>();

    public async Task<int> UpdateTeamAsync(Team team) => await _db.UpdateAsync(team);

    public async Task<List<Team>> GetTeamAsync() => await _db.Table<Team>().ToListAsync();

    public async Task<Team> GetTeamHomeAsync() => await _db.Table<Team>().Where(x => x.IsHome).FirstOrDefaultAsync();

    public async Task<Team> GetTeamGuestAsync() => await _db.Table<Team>().Where(x => !x.IsHome).FirstOrDefaultAsync();

    #endregion

    #region LineUpBegin 

    public async Task InitializeLineUpBeginAsync() => await _db.CreateTableAsync<LineUpBegin>();

    public async Task<int> SaveLineUpBeginAsync(LineUpBegin lineup) => await _db.InsertAsync(lineup);

    public async Task<int> DeleteLineUpBeginAsync() => await _db.DeleteAllAsync<LineUpBegin>();

    public async Task<List<LineUpBegin>> GetLineUpBeginAsync() => await _db.Table<LineUpBegin>().ToListAsync();

    public async Task<LineUpBegin> GetLineUpBeginAsync(Set set) => await _db.Table<LineUpBegin>().Where(x => x.SetId == set.Id).FirstOrDefaultAsync();

    public async Task<LineUpBegin> GetLineUpBeginAsync(Set set, Team team) => await _db.Table<LineUpBegin>().Where(x => x.SetId == set.Id && x.TeamId == team.Id).FirstOrDefaultAsync();

    #endregion

    #region Event

    public async Task InitializeEventAsync() => await _db.CreateTableAsync<Event>();

    public async Task<int> SaveEventAsync(Event ev) => await _db.InsertAsync(ev);

    public async Task<int> UpdateEventAsync(Event ev) => await _db.UpdateAsync(ev);

    public async Task<int> DeleteSelectEventAsync(Event ev) => await _db.DeleteAsync(ev);

    public async Task<int> DeleteEventAsync() => await _db.DeleteAllAsync<Event>();

    public async Task<List<Event>> GetEventAsync() => await _db.Table<Event>().ToListAsync();

    public async Task<List<Event>> GetEventAsync(Set set) => await _db.Table<Event>().Where(x => x.SetID == set.Id).ToListAsync();

    public async Task<List<Event>> GetEventAsync(Set set, List<int> IDs_events) => await _db.Table<Event>().Where(x => x.SetID == set.Id && IDs_events.Contains(x.EventID)).ToListAsync();

    public async Task<List<Event>> GetEventAsync(Team team, List<int> IDs_events) => await _db.Table<Event>().Where(x => x.TeamID == team.Id && IDs_events.Contains(x.EventID)).ToListAsync();

    public async Task<List<Event>> GetEventAsync(Set set, Team team, List<int> IDs_events) => await _db.Table<Event>().Where(x => x.SetID == set.Id && x.TeamID == team.Id && IDs_events.Contains(x.EventID)).ToListAsync();

    #endregion

    #region Sanction

    public async Task InitializeSanctionAsync() => await _db.CreateTableAsync<Sanction>();

    public async Task<int> SaveSanctionAsync(Sanction sanction) => await _db.InsertAsync(sanction);

    public async Task<int> UpdateSanctionAsync(Sanction sanction) => await _db.UpdateAsync(sanction);

    public async Task<int> DeleteSelectSanctionAsync(Sanction sanction) => await _db.DeleteAsync(sanction);

    public async Task<List<Sanction>> GetSanctionAsync() => await _db.Table<Sanction>().ToListAsync();

    public async Task<int> DeleteSanctionAsync() => await _db.DeleteAllAsync<Sanction>();

    #endregion
}