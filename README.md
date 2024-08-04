# Sistema de Gestión de la Escuela ACME

## Descripción

Este proyecto es una Prueba de Concepto (PoC) para un sistema de gestión de cursos y estudiantes para la escuela ACME. Actualmente, permite registrar estudiantes y cursos, inscribir estudiantes en cursos (con o sin pago) y obtener una lista de cursos dentro de un rango de fechas.

## Arquitectura

El proyecto sigue los principios de Clean Architecture y Domain-Driven Design (DDD), lo que garantiza una clara separación de responsabilidades y un enfoque en el dominio del problema. 

Las capas principales son:

- **Dominio (Domain):** Contiene las entidades principales del negocio, Value Objects y reglas de negocio del sistema.
- **Aplicación (Application):** Contiene los casos de uso que representan las acciones que los usuarios pueden realizar.
- **Infraestructura (Infrastructure):** (Pendiente de implementación) Contendrá las implementaciones concretas de los repositorios y servicios externos.

## Tecnologías Utilizadas

- C#
- xUnit.net (para pruebas unitarias)
- Moq (para crear mocks en las pruebas)
- AutoFixture (para generar datos de prueba)
- FluentAssertions (para hacer aserciones más legibles)

## Consideraciones

- **Persistencia:** Actualmente, no hay persistencia de datos. Si el PoC es aprobado, se implementará la capa de infraestructura para persistir los datos en una base de datos (el tipo de base de datos aún no está definido).
- **Pasarela de Pago:** La integración con una pasarela de pago real está pendiente. Se penso el proceso de pagos separado en dos partes: 1-Enviar los datos con un callbak para iniciar el pago en un servicio externo. 2-Se continua el proceso de inscripcion al curso cuando regrese por un callbak una vez procesado el pago en el servicio externo.
- **API:** El sistema no está expuesto como una API. Si el PoC es aprobado, se desarrollará una API para acceder a las funcionalidades del sistema.

## Cosas que me hubiera gustado hacer pero no hice
- Analizar mas posibilidades de Refactorizacion de código.
- Analizar si se cumple correctamente los principios SOLID en todos los escenarios.
- Agregar más pruebas unitarias y de integración para cubrir todos los escenarios posibles.
- Implementar logging y manejo de errores más robusto.
- Analizar la estructura en detalle para ver si es neceario plantear cambios en carpetas o capas.

## Bibliotecas de Terceros

- Moq: Para crear mocks en las pruebas unitarias.
- AutoFixture: Para generar datos de prueba automáticamente.
- FluentAssertions: Para hacer aserciones más legibles y expresivas en las pruebas.

## Aprendizajes

- Ajustes: Modificaciones claves luego de hacer los tests y notar mejoras posibles.
- Analisis: Encontrar la manera de plantearla pasarela de Pagos de una manera convincente