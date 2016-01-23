using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public abstract class iProgessData {
    public string Location { get { return GetLocation(); } set {;} }
    protected abstract string GetLocation();
    public abstract XmlDocument Save();
    public abstract void Load(XmlDocument document);

    protected XmlDocument AutoSerialize<T>(object instance) {
        XmlDocument xmlDocument = null;
        XmlSerializer serializer = new XmlSerializer(typeof(T));

        using (MemoryStream memStm = new MemoryStream()) {
            serializer.Serialize(memStm, instance);
            memStm.Position = 0;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (var xtr = XmlReader.Create(memStm, settings)) {
                xmlDocument = new XmlDocument();
                xmlDocument.Load(xtr);
            }
        }

        return xmlDocument;
    }

    public T AutoDeserialize<T>(XmlDocument document) {
        var serializer = new XmlSerializer(typeof(T));
        using (TextReader reader = new StringReader(document.OuterXml)) {
            return (T)serializer.Deserialize(reader);
        }
    }
}

public class Progress {
    private static List<iProgessData> progressors = new List<iProgessData>();

    public static void Save() {
        Debug.Log("[save]");
        foreach (iProgessData progressor in progressors) {
            XmlDocument document = progressor.Save();

            string str= Cryptographer.Encrypt(document.OuterXml);
            XmlDocument encryptedDoc = new XmlDocument();
            XmlNode node = encryptedDoc.CreateElement("data");
            node.InnerText = str;
            encryptedDoc.AppendChild(node);
            encryptedDoc.Save(Path.Combine(Application.persistentDataPath, progressor.Location));
        }
    }

    public static void Load() {
        Debug.Log("[load]");
        foreach (iProgessData progressor in progressors) {
            XmlDocument doc = new XmlDocument();
            string path = Path.Combine(Application.persistentDataPath, progressor.Location);
            if (File.Exists(path) == false) {
                continue;
            }
            doc.Load(path);
            doc.LoadXml(Cryptographer.Decrypt(doc["data"].InnerText));
            progressor.Load(doc);
        }
    }

    public static void Clear() {
        Debug.Log("[clear]");
        foreach (iProgessData progressor in progressors) {
            string path = Path.Combine(Application.persistentDataPath, progressor.Location);
            if (File.Exists(path) == false) {
                continue;
            }
            File.Delete(path);
        }
    }

    public static void AddProgressor(iProgessData progressor) {
        Debug.Log("[progressor-add-" + progressor.GetType() + "]");
        for(int i = 0; i < progressors.Count; i++) {
            iProgessData pd = progressors[i];
            if((pd.GetType()) == (progressor.GetType())) {
                progressors[i] = progressor;
                return;
            }
        }

        progressors.Add(progressor);

        string path = Path.Combine(Application.persistentDataPath, progressor.Location);
        if (File.Exists(path) == false) {
            return;
        }
        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        doc.LoadXml(Cryptographer.Decrypt(doc["data"].InnerText));
        progressor.Load(doc);
    }
}