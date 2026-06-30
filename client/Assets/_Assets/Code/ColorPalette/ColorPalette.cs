using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class ColorPalette : ScriptableObject 
{
	[SerializeField, HideInInspector] private SerializeableColor _main;
	[SerializeField, HideInInspector] private SerializeableColor _sub;
	[SerializeField, HideInInspector] private SerializeableColor _wall;
	[SerializeField, HideInInspector] private SerializeableColor _pattern;
	
	[JsonIgnore] public Color main
	{
		get{
			if(_main == null)
				_main = new SerializeableColor();
			return _main.color;
		}
		set{
			if(_main == null)
				_main = new SerializeableColor();

			_main.color = value;
		}
	}
	[JsonIgnore] public Color floor
	{
		get{
			if(_sub == null)
				_sub = new SerializeableColor();
			return _sub.color;
		}
		set{
			if(_sub == null)
				_sub = new SerializeableColor();

			_sub.color = value;
		}
	}
	[JsonIgnore] public Color wall
	{
		get{
			if(_wall == null)
				_wall = new SerializeableColor();
			return _wall.color;
		}
		set{
			if(_wall == null)
				_wall = new SerializeableColor();
			_wall.color = value;
		}
	}
	[JsonIgnore] public Color pattern
	{
		get{
			if(_pattern == null)
				_pattern = new SerializeableColor();
			return _pattern.color;
		}
		set{
			if(_pattern == null)
				_pattern = new SerializeableColor();
			_pattern.color = value;
		}
	}

	public SerializableData GetSerializableData()
	{
		SerializableData serializableData = new SerializableData();
		serializableData.main = _main;
		serializableData.sub = _sub;
		serializableData.wall = _wall;
		serializableData.pattern = _pattern;
		return serializableData;
	}

	public void Deserialize(ColorPalette.SerializableData serializableData)
	{
		_main = serializableData.main;
		_sub = serializableData.sub;
		_wall = serializableData.wall;
		_pattern = serializableData.pattern;
	}

	public class SerializableData
	{
		public SerializeableColor main;
		public SerializeableColor sub;
		public SerializeableColor wall;
		public SerializeableColor pattern;
	}
}


