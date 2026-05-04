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
        & await _db.DeleteAllAsync<LineUpBegin>() 
        & await _db.DeleteAllAsync<Team>() 
        & await _db.DeleteAllAsync<MainInformation>();

    #region MainInfo

    public async Task InitializeMainInfoAsync()
    {
        await _db.CreateTableAsync<MainInformation>();
    }

    public async Task<int> SaveMainInfoAsync(MainInformation info) => await _db.InsertAsync(info);

    public async Task<List<MainInformation>> GetMainInfoAsync() => await _db.Table<MainInformation>().ToListAsync();

    public async Task<int> DeleteMainInfoAsync() => await _db.DeleteAllAsync<MainInformation>();

    #endregion

    #region Roster

    public async Task InitializeRosterAsync()
    {
        await _db.CreateTableAsync<Player>();
    }

    public async Task<int> SaveRosterAsync(Player player) => await _db.InsertAsync(player);

    public async Task<List<Player>> GetRosterAsync() => await _db.Table<Player>().ToListAsync();

    public async Task<int> DeleteRosterAsync() => await _db.DeleteAllAsync<Player>();

    #endregion

    #region Set

    public async Task InitializeSetAsync()
    {
        await _db.CreateTableAsync<Set>();
    }

    public async Task<int> SaveSetAsync(Set set) => await _db.InsertAsync(set);

    public async Task<List<Set>> GetSetAsync() => await _db.Table<Set>().ToListAsync();

    public async Task<int> DeleteSetAsync() => await _db.DeleteAllAsync<Set>();

    #endregion

    #region Team

    public async Task InitializeTeamAsync()
    {
        await _db.CreateTableAsync<Team>();
    }

    public async Task<int> SaveTeamAsync(Team team) => await _db.InsertAsync(team);

    public async Task<List<Team>> GetTeamAsync() => await _db.Table<Team>().ToListAsync();

    public async Task<int> DeleteTeamAsync() => await _db.DeleteAllAsync<Team>();

    #endregion

    #region LineUp

    public async Task InitializeLineUpBeginAsync()
    {
        await _db.CreateTableAsync<LineUpBegin>();
    }

    public async Task<int> SaveLineUpAsync(LineUpBegin lineup) => await _db.InsertAsync(lineup);

    public async Task<List<LineUpBegin>> GetLineUpAsync() => await _db.Table<LineUpBegin>().ToListAsync();

    public async Task<int> DeleteLineUpAsync() => await _db.DeleteAllAsync<LineUpBegin>();

    #endregion

    #region Event

    public async Task InitializeEventAsync()
    {
        await _db.CreateTableAsync<Event>();
    }

    public async Task<int> SaveEventpAsync(Event ev) => await _db.InsertAsync(ev);

    public async Task<List<Event>> GetEventAsync() => await _db.Table<Event>().ToListAsync();

    public async Task<int> DeleteEventAsync() => await _db.DeleteAllAsync<Event>();

    #endregion

}