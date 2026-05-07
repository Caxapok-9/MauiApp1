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

    public async Task<int> DeleteAsync() =>
        await _db.DeleteAllAsync<Event>()
        & await _db.DeleteAllAsync<Set>()
        & await _db.DeleteAllAsync<Player>()
        & await _db.DeleteAllAsync<LineUp>()
        & await _db.DeleteAllAsync<Team>()
        & await _db.DeleteAllAsync<MainInformation>()
        & await _db.DeleteAllAsync<EventCategory>();

    #region MainInfo

    public async Task InitializeMainInfoAsync()
    {
        await _db.CreateTableAsync<MainInformation>();

        await _db.CreateTableAsync<MainInformation>();
    }

    public async Task<int> SaveMainInfoAsync(MainInformation info) => await _db.InsertAsync(info);

    public async Task<List<MainInformation>> GetMainInfoAsync() => await _db.Table<MainInformation>().ToListAsync();

    public async Task<int> DeleteMainInfoAsync() => await _db.DeleteAllAsync<MainInformation>();

    #endregion

    #region EventCategory

    public async Task InitializeEventCategoryAsync()
    {
        await _db.CreateTableAsync<EventCategory>();

        await _db.DeleteAllAsync<EventCategory>();

        var list = new List<EventCategory>
        {
            new EventCategory() {NameCategory = "Очко"},
            new EventCategory() {NameCategory = "Тайм-аут"},
            new EventCategory() {NameCategory = "Замена"}
        };

        await _db.InsertAllAsync(list);
    }

    public async Task<List<EventCategory>> GetEventCategoryAsync() => await _db.Table<EventCategory>().ToListAsync();

    public async Task<int> DeleteEventCategoryAsync() => await _db.DeleteAllAsync<EventCategory>();

    #endregion

    #region Roster

    public async Task InitializeRosterAsync()
    {
        await _db.CreateTableAsync<Player>();

        await _db.DeleteAllAsync<Player>();
    }

    public async Task<int> SaveRosterAsync(Player player) => await _db.InsertAsync(player);

    public async Task<List<Player>> GetRosterAsync() => await _db.Table<Player>().ToListAsync();

    public async Task<int> DeleteRosterAsync() => await _db.DeleteAllAsync<Player>();

    #endregion

    #region Set

    public async Task InitializeSetAsync()
    {
        await _db.CreateTableAsync<Set>();

        await _db.DeleteAllAsync<Set>();
    }

    public async Task<int> SaveSetAsync(Set set) => await _db.InsertAsync(set);

    public async Task<List<Set>> GetSetAsync() => await _db.Table<Set>().ToListAsync();

    public async Task<int> DeleteSetAsync() => await _db.DeleteAllAsync<Set>();

    public async Task<int> UpdateSetAsync(Set set) => await _db.UpdateAsync(set); 

    #endregion

    #region Team

    public async Task InitializeTeamAsync()
    {
        await _db.CreateTableAsync<Team>();

        await _db.DeleteAllAsync<Team>();
    }

    public async Task<int> SaveTeamAsync(Team team) => await _db.InsertAsync(team);

    public async Task<List<Team>> GetTeamAsync() => await _db.Table<Team>().ToListAsync();

    public async Task<int> DeleteTeamAsync() => await _db.DeleteAllAsync<Team>();

    public async Task<int> UpdateTeamAsync(Team team) => await _db.UpdateAsync(team);

    #endregion

    #region LineUp

    public async Task InitializeLineUpBeginAsync()
    {
        await _db.CreateTableAsync<LineUp>();

        await _db.DeleteAllAsync<LineUp>();
    }

    public async Task<int> SaveLineUpAsync(LineUp lineup) => await _db.InsertAsync(lineup);

    public async Task<List<LineUp>> GetLineUpAsync() => await _db.Table<LineUp>().ToListAsync();

    public async Task<int> DeleteLineUpAsync() => await _db.DeleteAllAsync<LineUp>();

    #endregion

    #region Event

    public async Task InitializeEventAsync()
    {
        await _db.CreateTableAsync<Event>();

        await _db.DeleteAllAsync<Event>();
    }

    public async Task<int> SaveEventAsync(Event ev) => await _db.InsertAsync(ev);

    public async Task<List<Event>> GetEventAsync() => await _db.Table<Event>().ToListAsync();

    public async Task<int> DeleteEventAsync() => await _db.DeleteAllAsync<Event>();

    #endregion

}