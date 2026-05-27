using MauiApp1;
using SQLite;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.ExecuteAsync("PRAGMA foreign_keys = OFF;");
    }

    public Dictionary<string, int> EventsCategories;

    public List<SanctionCategory> SanctionsCategories;
    public async Task ClearAsync()
    {
        await _db.DeleteAllAsync<Event>();
        await _db.DeleteAllAsync<LineUpBegin>();
        await _db.DeleteAllAsync<Set>();
        await _db.DeleteAllAsync<MainInformation>();
        await _db.DeleteAllAsync<Player>();
        await _db.DeleteAllAsync<Team>();
        await _db.DeleteAllAsync<EventCategory>();
        await _db.DeleteAllAsync<SanctionCategory>();
        await _db.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name IN ('Event', 'LineUpBegin', 'Set', 'MainInformation', 'Player', 'Team');");
    }

    public async Task InizializeAllTablesAsync()
    {
        await _db.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS ""Set"" (
            ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT, 
            ""NumberSet"" INTEGER NOT NULL, 
            ""WinnerID"" INTEGER, 
            ""IsShort"" INTEGER NOT NULL DEFAULT 0, 
            FOREIGN KEY (""WinnerID"") REFERENCES ""Team""(""ID""));");

        await _db.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS ""Team"" (
            ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT, 
            ""Name"" TEXT NOT NULL, 
            ""IsHome"" INTEGER NOT NULL, 
            ""FirstSetServ"" INTEGER NOT NULL DEFAULT 0, 
            ""FinalySetServ"" INTEGER NOT NULL DEFAULT 0, 
            ""IsLeft"" INTEGER NOT NULL);");

        await _db.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS ""LineUpBegin"" (
            ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT, 
            ""SetID"" INTEGER, 
            ""TeamID"" INTEGER, 
            ""Zone1PlayerID"" INTEGER, 
            ""Zone2PlayerID"" INTEGER, 
            ""Zone3PlayerID"" INTEGER, 
            ""Zone4PlayerID"" INTEGER, 
            ""Zone5PlayerID"" INTEGER, 
            ""Zone6PlayerID"" INTEGER, 
            FOREIGN KEY (""SetID"") REFERENCES ""Set""(""ID""), 
            FOREIGN KEY (""TeamID"") REFERENCES ""Team""(""ID""), 
            FOREIGN KEY (""Zone1PlayerID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""Zone2PlayerID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""Zone3PlayerID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""Zone4PlayerID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""Zone5PlayerID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""Zone6PlayerID"") REFERENCES ""Player""(""ID""));");

        await _db.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS ""Player"" (
            ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT, 
            ""Name"" TEXT NOT NULL, 
            ""Number"" TEXT NOT NULL, 
            ""TeamID"" INTEGER, 
            ""ReplaceID"" INTEGER, 
            ""IsDisqual"" INTEGER NOT NULL DEFAULT 0, 
            ""IsRemove"" INTEGER NOT NULL DEFAULT 0, 
            ""IsInjury"" INTEGER NOT NULL DEFAULT 0, 
            ""IsCaptain"" INTEGER NOT NULL DEFAULT 0, 
            ""IsLibero"" INTEGER NOT NULL DEFAULT 0, 
            ""IsCoach"" INTEGER NOT NULL DEFAULT 0, 
            FOREIGN KEY (""TeamID"") REFERENCES ""Team""(""ID""), 
            FOREIGN KEY (""ReplaceID"") REFERENCES ""Player""(""ID""));");

        await _db.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS ""Event"" (
            ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT, 
            ""SetID"" INTEGER, 
            ""TeamID"" INTEGER, 
            ""EventCategoryID"" INTEGER, 
            ""ScoreHome"" INTEGER NOT NULL, 
            ""ScoreGuest"" INTEGER NOT NULL, 
            ""PlayerInID"" INTEGER, 
            ""PlayerOutID"" INTEGER, 
            ""SanctionCategoryID"" INTEGER, 
            ""TargetID"" INTEGER, 
            FOREIGN KEY (""SetID"") REFERENCES ""Set""(""ID""), 
            FOREIGN KEY (""TeamID"") REFERENCES ""Team""(""ID""), 
            FOREIGN KEY (""EventCategoryID"") REFERENCES ""EventCategory""(""ID""), 
            FOREIGN KEY (""PlayerInID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""PlayerOutID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""SanctionCategoryID"") REFERENCES ""SanctionCategory""(""ID""), 
            FOREIGN KEY (""TargetID"") REFERENCES ""Player""(""ID""));");

        await _db.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS ""MainInformation"" (
            ""ID"" INTEGER PRIMARY KEY AUTOINCREMENT, 
            ""NameTournament"" TEXT NOT NULL, 
            ""TeamHomeID"" INTEGER, 
            ""TeamGuestID"" INTEGER, 
            ""FirstReferee"" TEXT NOT NULL, 
            ""ToReferee"" TEXT, 
            ""Secretary"" TEXT NOT NULL, 
            ""Group"" TEXT, 
            ""TimeBegin"" TEXT, 
            ""MVPHomeID"" INTEGER, 
            ""MVPGuestID"" INTEGER, 
            ""End"" INTEGER NOT NULL DEFAULT 0, 
            ""TextProtestHome"" TEXT, 
            ""TextProtestGuest"" TEXT, 
            ""TextProtestSecretary"" TEXT, 
            ""TextProtestFirstReferee"" TEXT, 
            ""TextProtestToReferee"" TEXT, 
            FOREIGN KEY (""TeamHomeID"") REFERENCES ""Team""(""ID""), 
            FOREIGN KEY (""TeamGuestID"") REFERENCES ""Team""(""ID""), 
            FOREIGN KEY (""MVPHomeID"") REFERENCES ""Player""(""ID""), 
            FOREIGN KEY (""MVPGuestID"") REFERENCES ""Player""(""ID""));");

        await InitializeEventCategoryAsync();
        await InitializeSanctionCategoryAsync();
    }

    public int GetIdSanctionByName(string name)
    {
        return SanctionsCategories.Find(x => x.Name == name).ID;
    }

    public async Task<List<Player>> GetRosterFull(Team team)
    {
        return await GetPlayerAsync(team);
    }

    public async Task<List<Player>> GetRosterAccess(Team team)
    {
        var list = await GetPlayerAsync(team);

        return list.Where(x => !x.IsDisqual && !x.IsInjury && !x.IsRemove).ToList();
    }

    public async Task<List<Player>> GetRosterPlayer(Team team)
    {
        var list = await GetPlayerAsync(team);

        return list.Where(x => !x.IsDisqual && !x.IsInjury && !x.IsRemove && !x.IsLibero && !x.IsCoach).ToList();
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

        var evcat = await _db.Table<EventCategory>().ToListAsync();

        if (evcat.Count == 0)
        {
            var list = new List<EventCategory>
            {
                new EventCategory() {Name = "SC"},
                new EventCategory() {Name = "T"},
                new EventCategory() {Name = "R"},
                new EventCategory() {Name = "RR"},
                new EventCategory() {Name = "ER"},
                 new EventCategory() {Name = "SA"}
            };

            await _db.InsertAllAsync(list);
        }

        var l = await _db.Table<EventCategory>().ToListAsync();

        EventsCategories = l.ToDictionary(x => x.Name, x => x.ID);
    }

    #endregion

    #region SanctionCategory

    public async Task InitializeSanctionCategoryAsync()
    {
        await _db.CreateTableAsync<SanctionCategory>();

        var sancat = await _db.Table<SanctionCategory>().ToListAsync();

        if (sancat.Count == 0)
        {
            var list = new List<SanctionCategory>
            {
                new SanctionCategory() {Name = "Warning", DisplayName = "Предупреждение (Жёлтая карточка)"},
                new SanctionCategory() {Name = "Remark", DisplayName = "Замечание (Красная карточка)"},
                new SanctionCategory() {Name = "Remove", DisplayName = "Удаление (Две карточки в одной руке)"},
                new SanctionCategory() {Name = "Disqual", DisplayName = "Дисквалификация (Две карточки в двух руках)"}
            };

            await _db.InsertAllAsync(list);
        }

        SanctionsCategories = await _db.Table<SanctionCategory>().ToListAsync();
    }

    #endregion

    #region Player

    public async Task InitializePlayerAsync() => await _db.CreateTableAsync<Player>();

    public async Task ClearReplaceID() => await _db.ExecuteAsync("UPDATE Player SET ReplaceID = 0", false);

    public async Task ClearRemove() => await _db.ExecuteAsync("UPDATE Player SET IsRemove = FALSE", false);

    public async Task SavePlayerAsync(Player player) => await _db.InsertAsync(player);

    public async Task UpdatePlayerAsync(Player player)
    {
        await _db.UpdateAsync(player);
    }

    public async Task<List<Player>> GetPlayerAsync() => await _db.Table<Player>().ToListAsync();

    public async Task<Player> GetPlayerAsync(int id) => await _db.Table<Player>().Where(x => x.ID == id).FirstOrDefaultAsync();

    public async Task<List<Player>> GetPlayerAsync(Team team) => await _db.Table<Player>().Where(x => x.TeamID == team.ID).ToListAsync();

    public async Task<int> DeletePlayerAsync() => await _db.DeleteAllAsync<Player>();

    #endregion

    #region Set

    public async Task InitializeSetAsync() => await _db.CreateTableAsync<Set>();

    public async Task<int> SaveSetAsync(Set set) => await _db.InsertAsync(set);

    public async Task<int> DeleteSetAsync() => await _db.DeleteAllAsync<Set>();

    public async Task<int> UpdateSetAsync(Set set) => await _db.UpdateAsync(set);

    public async Task<List<Set>> GetSetAsync() => await _db.Table<Set>().ToListAsync();

    public async Task<Set> GetSetAsync(int ID) => await _db.Table<Set>().Where(x => x.ID == ID).FirstOrDefaultAsync();

    public async Task<Set> GetLastSetAsync() => await _db.Table<Set>().OrderByDescending(x => x.ID).FirstOrDefaultAsync();

    #endregion

    #region Team

    public async Task InitializeTeamAsync() => await _db.CreateTableAsync<Team>();

    public async Task<int> SaveTeamAsync(Team team) => await _db.InsertAsync(team);

    public async Task<int> DeleteTeamAsync() => await _db.DeleteAllAsync<Team>();

    public async Task<int> UpdateTeamAsync(Team team) => await _db.UpdateAsync(team);

    public async Task<List<Team>> GetTeamAsync() => await _db.Table<Team>().ToListAsync();

    public async Task<Team> GetTeamAsync(int id) => await _db.Table<Team>().Where(x => x.ID == id).FirstOrDefaultAsync();

    public async Task<Team> GetTeamHomeAsync() => await _db.Table<Team>().Where(x => x.IsHome).FirstOrDefaultAsync();

    public async Task<Team> GetTeamGuestAsync() => await _db.Table<Team>().Where(x => !x.IsHome).FirstOrDefaultAsync();

    #endregion

    #region LineUpBegin 

    public async Task InitializeLineUpBeginAsync() => await _db.CreateTableAsync<LineUpBegin>();

    public async Task<int> SaveLineUpBeginAsync(LineUpBegin lineup) => await _db.InsertAsync(lineup);

    public async Task<int> DeleteLineUpBeginAsync() => await _db.DeleteAllAsync<LineUpBegin>();

    public async Task<List<LineUpBegin>> GetLineUpBeginAsync() => await _db.Table<LineUpBegin>().ToListAsync();

    public async Task<LineUpBegin> GetLineUpBeginAsync(Set set) => await _db.Table<LineUpBegin>().Where(x => x.SetID == set.ID).FirstOrDefaultAsync();

    public async Task<LineUpBegin> GetLineUpBeginAsync(Set set, Team team) => await _db.Table<LineUpBegin>().Where(x => x.SetID == set.ID && x.TeamID == team.ID).FirstOrDefaultAsync();

    #endregion

    #region Event

    public async Task InitializeEventAsync() => await _db.CreateTableAsync<Event>();

    public async Task<int> SaveEventAsync(Event ev) => await _db.InsertAsync(ev);

    public async Task<int> UpdateEventAsync(Event ev) => await _db.UpdateAsync(ev);

    public async Task<int> DeleteSelectEventAsync(Event ev) => await _db.DeleteAsync(ev);

    public async Task<int> DeleteEventAsync() => await _db.DeleteAllAsync<Event>();

    public async Task<Event> GetLastEventAsync()
    {
        var ev = await _db.Table<Event>().OrderByDescending(x => x.ID).FirstOrDefaultAsync();

        return ev != null ? ev : null;
    }

    public async Task<List<Event>> GetEventAsync(List<int> IDs_events = null)
    {
        if(IDs_events != null)
            return await _db.Table<Event>().Where(x => IDs_events.Contains(x.EventCategoryID)).ToListAsync();
        else
            return await _db.Table<Event>().ToListAsync();
    }

    public async Task<List<Event>> GetEventAsync(Set set) => await _db.Table<Event>().Where(x => x.SetID == set.ID).ToListAsync();

    public async Task<List<Event>> GetEventAsync(Set set, List<int> IDs_events) => await _db.Table<Event>().Where(x => x.SetID == set.ID && IDs_events.Contains(x.EventCategoryID)).ToListAsync();

    public async Task<List<Event>> GetEventAsync(Team team, List<int> IDs_events) => await _db.Table<Event>().Where(x => x.TeamID == team.ID && IDs_events.Contains(x.EventCategoryID)).ToListAsync();

    public async Task<List<Event>> GetEventAsync(Set set, Team team, List<int> IDs_events) => await _db.Table<Event>().Where(x => x.SetID == set.ID && x.TeamID == team.ID && IDs_events.Contains(x.EventCategoryID)).ToListAsync();

    public async Task<Tuple<int, int>> GetScore(Set set)
    {
        var events = await _db.Table<Event>().ToListAsync();

        if (events != null && events.Count > 0)
        {
            Team TeamHome = await GetTeamHomeAsync();

            Team TeamGuest = await GetTeamGuestAsync();

            int scoreHome = events.Where(x => x.SetID == set.ID && x.TeamID == TeamHome.ID && x.EventCategoryID == EventsCategories["SC"]).Count();

            int scoreGuest = events.Where(x => x.SetID == set.ID && x.TeamID == TeamGuest.ID && x.EventCategoryID == EventsCategories["SC"]).Count();

            return Tuple.Create(scoreHome, scoreGuest);
        }
        else
        {
            return Tuple.Create(0, 0);
        }
    }

    #endregion
}