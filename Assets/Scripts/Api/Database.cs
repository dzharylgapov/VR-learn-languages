using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class Database : MonoBehaviour
{
    private IDbConnection dbconn;
    private IDbCommand dbcmd;
    private string conn;
    List<object> result = new List<object>();
    List<string> nameOfDialogues = new List<string>();
    List<long> idOfDialogues = new List<long>();

    public List<object> Result { get => result; set => result = value; }
    public List<string> NameOfDialogues { get => nameOfDialogues; set => nameOfDialogues = value; }
    public List<long> IdOfDialogues { get => idOfDialogues; set => idOfDialogues = value; }

    private void Start()
    {
        conn = "URI=file:" + Application.dataPath + "/db.db";

        LoadDialogues();
    }

    private void LoadDialogues()
    {
        ExecuteCommand(
            "SELECT nameOfDialogue FROM Dialogue;"
        );
        foreach (var elem in Result)
        {
            NameOfDialogues.Add((string) elem);
        }
        ClearResult();
        ExecuteCommand(
            "SELECT id FROM Dialogue;"
        );
        foreach (var elem in Result)
        {
            IdOfDialogues.Add((long)elem);
        }
        ClearResult();

    }

    public void ExecuteCommand(string sqlQuery)
    {
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        dbcmd = dbconn.CreateCommand();

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            Result.Add(reader.GetValue(0));

            Debug.Log("value= " + Result);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public void ClearResult()
    {
        result = new List<object>();
    }
}
