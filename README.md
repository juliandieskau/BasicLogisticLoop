# BasicLogisticLoop
## Design-Entscheidungen
- Ist nur eine Basic-Version -> Erweiterbarkeit für Ergänzungen oder Veränderungen am Modell
	- Gegen Interfaces implementiert, sodass man das Modell durch ein neues ersetzen kann
	- "View Logik" lässt sich wegen Windows Forms nicht gut ersetzen 
	-> Wie etwas angezeigt wird aus dem Modell müsste man dann im Forms auch anpassen (weitere Methoden)
- MVP für Trennung zwischen Modell und Benutzerinteraktion
	- Windows Forms ist nicht wirklich für MVP geeignet und hat schon die Hauptschleife vom Programm
	-> Presenter kann nicht die tatsächliche Kontrolle haben, wird aber immer von View mit den reinen Informationen angefragt und behandelt alles vom Modell
	- Nach MVP soll das Modell nicht vom View "wissen" und View das Modell nicht direkt verändern können
	-> ViewNode structs, die Value-Typen sind also als Kopie und nicht per Referenz übergeben werden mit den wichtigen Daten aus dem Modell, die View anzeigen soll
- Form Designer nur für Prototypen benutzt, ansonsten alles im Code erzeugen
	- Wegen Erweiterbarkeit, sobald man ein leicht anderes Modell hat muss man den Designer dafür verwenden aber kann immer nur ein Modell halten
	- Namenskonventionen sind übersichtlicher, Zugriff auf bestimmte Teile des View kann über Methoden und Konstanten bestimmt werden
	- Man hätte nur Modell dynamisch erzeugen können und den Rest außenrum zu Design-Zeit, aber ich habe es gerne einheitlich