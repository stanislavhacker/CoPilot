java -jar compiler.jar ^
	--js ../js/jquery-1-8-3min.js ^
    --js ../js/cp/copilot.app.js ^
	--js ../js/cp/data/copilot.data.language.js ^
	--js ../js/cp/data/copilot.data.renderer.js ^
	--js ../js/cp/data/copilot.data.skin.js ^
	--js ../js/cp/data/copilot.data.data.js ^
	--js_output_file ../cp.js

dotless.Compiler.exe -m ../css/cp.less ../cp.css