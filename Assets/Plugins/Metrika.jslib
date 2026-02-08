mergeInto(LibraryManager.library, {

	Analytics_Goal: function (eventName) {
		// VK 102903100
	
		let sendEvent = UTF8ToString(eventName);
		// sendEvent = `${sendEvent}_test`;
		
		ym(102903100,'reachGoal', sendEvent);
		//console.log(`reachGoal.${sendEvent}`);
    },

});