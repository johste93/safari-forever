using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WebWorkerFactory {

	public static WebWorker HireWorker(MonoBehaviour supervisor = null)
	{
		if(!Application.isPlaying)
		{
			Debug.LogError("Can only be used @Runtime");
			return null;
		}

		string name = "Web-Worker " + webWorkerNames[Random.Range(0, webWorkerNames.Length)] + " #" + Random.Range(100,1000);
		GameObject gameObject = new GameObject(name);
		gameObject.AddComponent<WebWorker>();
		WebWorker latestWorker = gameObject.GetComponent<WebWorker>();
		latestWorker.SetSupervisor(supervisor);
		return latestWorker;
	}

	private static string[] webWorkerNames = new string[]
	{
		"David", "John", "Andrew", 
		"James","Christopher","Paul","Steven","Kevin","Robert","Scott","Craig","Michael","Mark","Stuart","Stephen","Alan","William","Gary","Ross","Colin","Brian","Barry","Richard","Martin","Thomas","Neil","Peter","Iain","Ian","Gordon","Alexander","Ryan","Derek","Kenneth","Allan","Jamie","Graham","Gavin","Darren","Stewart","Gareth","Jonathan","Daniel","Douglas","Grant","Lee","George","Joseph","Simon","Matthew","Keith","Anthony","Fraser","Garry","Alistair","Bryan","Philip","Adam","Sean","Duncan","Edward","Charles","Ewan","Russell","Donald","Patrick","Alastair","Euan","Jason","Nicholas","Marc","Raymond","Malcolm","Greig","Alasdair","Greg","Liam","Shaun","Francis","Ronald","Benjamin","Cameron","Dean","Niall","Gerard","Murray","Robin","Timothy","Bruce","Hugh","Calum","Kris","Wayne","Roderick","Samuel","Martyn","Angus","Gregor","Jon","Rory","Callum","Dale","Lewis","Antony","Aaron","Barrie","Jordan","Blair","Mohammed","Gerald","Roy","Justin","Kieran","Henry","Owen","Eric","Brendan","Norman","Robbie","Dominic","Kristopher","Alisdair","Bernard","Campbell","Leslie","Oliver","Terry","Frank","Gregg","Gregory","Mohammad","Adrian","Billy","Trevor","Ben","Phillip","Terence","Tony","Drew","Geoffrey","Harry","Luke","Nathan","Damien","Dennis","Dylan","Ewen","Kyle","Alister","Hamish","Jeffrey","Jonathon","Neal","Sandy","Arthur","Evan","Frazer","Jody","Karl","Kristofer","Leon","Marcus","Tom","Christian","Glen","Jack","Leigh","Mathew","Asif","Carl","Damian","Denis","Frederick","Guy","Kirk","Lawrence","Lindsay","Stefan","Aidan","Allen","Darran","Kerr","Kristoffer","Laurence","Vincent","Wesley","Alun","Bobby","Bradley","Derrick","Desmond","Fergus","Imran","Innes","Jay","Kevan","Lachlan","Neill","Nigel","Roger","Ruairidh","Abdul","Antonio","Ashley","Daryl","Finlay","Johnathan","Magnus","Marco","Maurice","Nicolas","Ricky","Ritchie","Tristan","Archibald","Austin","Brett","Giles","Gillon","Julian","Kieron","Lloyd","Maxwell","Muhammad","Murdo","Nicol","Ralph","Scot","Shane","Chun","Clark","Crawford","Don","Gillan","Jeremy","Jim","Jodie","Kristofor","Laurie","Ramsay","Ray","Rhys","Rikki","Ronan","Stanley","Steve","Tommy","Albert","Alex","Benedict","Clifford","Danny","Eamonn","Elliot","Erik","Garrie","Glenn","Glyn","Joe","Joshua","Lawrie","Leonard","Louis","Moray","Morgan","Omar","Roddy","Ronnie","Ruaridh","Russel","Sam","Stefano","Warren","Adnan","Amit","Bruno","Bryce","Chris","Clive","Darrell","Darron","Darryn","Edwin","Eion","Findlay","Garreth","Hector","Hugo","Isaac","Joel","Manus","Marshall","Myles","Nairn","Nicky","Nikki","Ranald","Reginald","Roland","Rudi","Shahid","Wai","Walter","Ahmad","Alec","Alfred","Allister","Arron","Asa","Asim","Austen","Barclay","Barnaby","Brent","Carlo","Clarke","Dante","Dario","Darrel","Darrin","Darroch","Darryl","Derick","Dougal","Duane","Gerrard","Gilbert","Giovanni","Grahame","Howard","Irfan","Ivan","Ivor","Jacob","Jaimie","Jake","Jed","Jerome","Kamran","Keir","Kristian","Leighton","Lorne","Marcos","Mario","Miles","Mitchell","Mohamed","Muhammed"
	};
}
