java -jar compiler.jar ^
	--js ../js/jquery-1-8-3min.js ^
	--js ../js/highcharts.js ^
	--js ../js/cp/copilot.app.js ^
	--js ../js/cp/data/copilot.data.language.js ^
	--js ../js/cp/data/copilot.data.map.js ^
	--js ../js/cp/data/copilot.data.renderer.js ^
	--js ../js/cp/data/copilot.data.skin.js ^
	--js ../js/cp/data/copilot.data.data.js ^
	--js ../js/cp/data/copilot.data.storage.js ^
	--js ../js/cp/model/copilot.model.settings.js ^
	--js ../js/cp/model/copilot.model.maintenances.js ^
	--js ../js/cp/model/copilot.model.fills.js ^
	--js ../js/cp/model/copilot.model.videos.js ^
	--js ../js/cp/model/copilot.model.images.js ^
	--js ../js/cp/model/copilot.model.repairs.js ^
	--js ../js/cp/model/copilot.model.states.js ^
	--js ../js/cp/model/copilot.model.data.js ^
	--js ../js/cp/model/copilot.model.graph.js ^
	--js ../js/cp/model/copilot.model.paths.js ^
	--js ../js/cp/model/copilot.model.skin.js ^
	--js ../js/cp/model/copilot.model.circuits.js ^
	--js ../js/cp/model/copilot.model.circuitGroup.js ^
	--js_output_file ../cp.js

dotless.Compiler.exe -m ../css/cp.less ../cp.css