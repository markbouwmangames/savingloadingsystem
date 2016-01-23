using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Core : iProgessData {
    //Player data
    public Player Player = new Player();

    //Employees
    public ProgrammerContainer ProgrammerContainer = new ProgrammerContainer();
    public ArtistContainer ArtistContainer = new ArtistContainer();
    //Misc employees
    public AudioEngineer AudioEngineer = new AudioEngineer();
    public Producer Producer = new Producer();

    //Progress 
    public Company Company = new Company();
    public Office Office = new Office();
    public UpgradeContainer UpgradeContainer = new UpgradeContainer();

    //Game
    public GameContainer GameContainer = new GameContainer();

    //Time
    public DateTime LastRandomDrop = new DateTime(1,1,1);
    public DateTime LastMinigamePlayed = new DateTime(1, 1, 1);
    public DateTime LastGameTick = new DateTime(1, 1, 1);

    //Logic getters
    public int EmployeeCount {
        get {
            return ProgrammerContainer.Amount + ArtistContainer.Amount + AudioEngineer.Amount + Producer.Amount;
        }
    }

    //Load save logic    
    [NonSerialized]
    private static Core _instance = null;
    public static Core Instance {
        get {
            if (_instance == null) {
                _instance = new Core();
                Progress.AddProgressor(_instance);
            } return _instance;
        }
        set {
            if (_instance == null) {
                _instance = value;
            }
        }
    }
    protected override string GetLocation() {
        return "core_test";
    }

    public override XmlDocument Save() {
        return AutoSerialize<Core>(this);
    }

    public override void Load(XmlDocument document) {
        Core tmp = AutoDeserialize<Core>(document);
        foreach (var property in GetType().GetFields()) {
            if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).GetLength(0) == 0)
                property.SetValue(this, property.GetValue(tmp));
        }
    }
}
