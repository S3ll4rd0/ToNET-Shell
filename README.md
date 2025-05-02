# ToNET-Shell

Una minishell en C# (.NET 8) creada por Tonet

El proyecto Minishell consiste en crear un shell simple similar a Bash, implementado en C# y .NET 8.

## Requerimientos Obligatorios:

**Prompt y Comandos**
* Mostrar un prompt esperando nuevos comandos usando `Console.WriteLine`/`ReadLine`
* Implementar historial funcional usando colecciones .NET
* Buscar y ejecutar ejecutables usando `System.Diagnostics.Process`

**Manejo de Señales**
* Implementar manejo de señales usando `Console.CancelKeyPress`
* Gestionar `ctrl-C`, `ctrl-D` y `ctrl-\` usando eventos del sistema

**Interpretación de Caracteres**
* Parser de comillas simples (`'`) usando expresiones regulares
* Parser de comillas dobles (`"`) con `string` interpolation
* Manejo de variables de entorno usando `Environment.GetEnvironmentVariable`
* Gestión de códigos de salida con `Environment.ExitCode`

**Redirecciones y Pipes**
* Implementar redirección de entrada usando `StreamReader`
* Implementar redirección de salida usando `StreamWriter`
* Implementar *here-document* con `StringBuilder`
* Implementar redirección append usando `File.AppendText`
* Implementar *pipes* usando `System.IO.Pipes`

**Comandos Built-in**
* `echo` con opción `-n` usando `Console.Write`
* `cd` usando `Directory.SetCurrentDirectory`
* `pwd` usando `Directory.GetCurrentDirectory`
* `export` usando `Environment.SetEnvironmentVariable`
* `unset` usando `Environment.SetEnvironmentVariable(var, null)`
* `env` usando `Environment.GetEnvironmentVariables`
* `exit` usando `Environment.Exit`

## Requerimientos del Bonus 🌟

* Implementar `&&` y `||` usando expresiones lambda y evaluación condicional
* Implementar *wildcards* `*` usando `Directory.EnumerateFiles`

## Reglas Generales

* Debe estar escrito en C# moderno (C# 12)
* Debe seguir los principios SOLID y convenciones de código .NET
* Debe implementar manejo de excepciones apropiado
* Debe incluir solución y proyecto en formato SDK style
* Debe usar características modernas de .NET 8
* Implementar tests unitarios usando xUnit/NUnit
* Documentación XML para IntelliSense

## Características Adicionales Recomendadas

* Usar `async`/`await` para operaciones asíncronas
* Implementar logging usando `ILogger`
* Usar inyección de dependencias
* Implementar configuración usando `IConfiguration`
* Usar `System.CommandLine` para parsing avanzado

**Nota importante:** La parte bonus solo será evaluada si la parte obligatoria está perfectamente implementada.

## Estado del Proyecto

🚧 En desarrollo

## Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.
