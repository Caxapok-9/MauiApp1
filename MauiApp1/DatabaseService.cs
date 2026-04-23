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

    public async Task InitializeMainInfoAsync()
    {
        await _db.CreateTableAsync<MainInformation>();
    }

    public async Task InitializeRosterAsync()
    {
        await _db.CreateTableAsync<PlayerHome>();
        await _db.CreateTableAsync<PlayerGuest>();
    }

    public async Task<int> SaveMainInfoAsync(MainInformation info) => await _db.InsertOrReplaceAsync(info);

    public async Task<int> SaveRosterHomeAsync(PlayerHome player) => await _db.InsertOrReplaceAsync(player);

    public async Task<int> SaveRosterGuestAsync(PlayerGuest player) => await _db.InsertOrReplaceAsync(player);

    public async Task<List<MainInformation>> GetMainInfoAsync() => await _db.Table<MainInformation>().ToListAsync();

    public async Task<List<PlayerHome>> GetRosterHomeAsync() => await _db.Table<PlayerHome>().ToListAsync();

    public async Task<List<PlayerGuest>> GetRosterGuestAsync() => await _db.Table<PlayerGuest>().ToListAsync();

    public async Task<int> DeleteMainInfoAsync() => await _db.DeleteAllAsync<MainInformation>();

    public async Task<int> DeleteRosterAsync() => await _db.DeleteAllAsync<PlayerHome>() & await _db.DeleteAllAsync<PlayerGuest>();
}