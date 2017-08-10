package Views

import javafx.scene.Parent
import javafx.scene.control.*
import javafx.scene.layout.*
import tornadofx.View
import tornadofx.*


class Popup:Fragment()
{
    override val root = VBox()

    init {
        with(root){
            label("kek")
        }
    }
}

class AppView: View() {
    override val root = AnchorPane()

    init {
        with(root) {
            textarea {
                text = "Hello world"

            }
        }
    }
}