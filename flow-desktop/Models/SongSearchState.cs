using System.Collections.Generic;

namespace flow_desktop.Models;

public abstract class SongSearchState
{
    public class Idle: SongSearchState {}
    public class Searching: SongSearchState {}
    public class NoResults: SongSearchState {}
    public class Error: SongSearchState {}

    public class FoundResults : SongSearchState
    {
        public List<SongSearchItem> SongSearchResults { get; }

        public FoundResults(
            List<SongSearchItem> songSearchResults
        ) => SongSearchResults = songSearchResults;
    }
}