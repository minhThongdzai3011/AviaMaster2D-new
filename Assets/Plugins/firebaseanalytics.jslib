mergeInto(LibraryManager.library, {
    SetUserProperties: function(properties) {
        var props = JSON.parse(UTF8ToString(properties));
		
        firebaseSetUserProperties(props);
    },
	
    LogEvent: function(eventName) {
        var event_name = UTF8ToString(eventName);
		
        firebaseLogEvent(event_name);
    },
	
    LogEventParameter: function(eventName, eventParameter) {
        var event_name = UTF8ToString(eventName);
        var event_param = JSON.parse(UTF8ToString(eventParameter));
		
        firebaseLogEventParameter(event_name, event_param);
    }
});