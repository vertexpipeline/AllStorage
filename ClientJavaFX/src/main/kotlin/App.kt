package main

import Views.AppView
import javafx.application.Application
import tornadofx.App

class App: App(AppView::class)

class MainClass {
    companion object {
        @JvmStatic fun main(args: Array<String>) {
            Application.launch(main.App::class.java)
        }
    }
}