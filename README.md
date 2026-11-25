# Trotamundos — Plataforma Móvil de Experiencias de Viaje

Aplicación móvil multiplataforma (**Android, iOS y Web**) desarrollada con **React Native** y respaldada por una API RESTful construida en **C# / .NET**, **Entity Framework Core** y **SQL Server**. **Trotamundos** está diseñada para viajeros que desean explorar destinos alrededor del mundo sin conocimiento previo del país, permitiéndoles planificar y guiarse a través de reseñas reales de otros usuarios, así como documentar y compartir sus propias experiencias para ayudar a la comunidad.

---

## Descripción funcional y módulos principales

- **Exploración global en 3D (Cesium):** Globo terráqueo interactivo donde el usuario selecciona cualquier país directamente sobre el mapa. Al hacer clic, se captura el código ISO (`ISO_A2` del GeoJSON de Natural Earth) y se resuelve dinámicamente contra la API (`GET /api/mapas/paises/iso/{codigo}`) para desplegar la información del destino.
- **Feed de experiencias por país y ciudad:** Listado filtrado de publicaciones creadas por otros viajeros con datos prácticos sobre destinos, cultura local, costos y puntos de interés.
- **Wizard de publicación en 6 pasos:** Flujo guiado y estructurado para compartir un viaje sin omitir detalles clave:
  1. **Ubicación** (país, ciudad y geolocalización)
  2. **Alojamiento** (hospedajes recomendados)
  3. **Comidas** (experiencias gastronómicas y restaurantes)
  4. **Recomendaciones** (consejos prácticos y agencias de turismo)
  5. **Fotos** (carga de galería multimedia)
  6. **Confirmar** (revisión final y publicación)
- **Gestión de cuenta y seguridad:** Registro, inicio de sesión con persistencia segura de tokens (`SecureStore` / `localStorage`), perfil de usuario, gestión de favoritos y flujo completo de recuperación de contraseña por correo electrónico.
- **Panel de administración y moderación:** Sistema de gestión de reportes de contenido, bloqueo de correos, sistema de invitaciones para nuevos administradores y control de roles (`Admin` y `SuperAdmin`).

---

## Arquitectura y stack tecnológico

### Backend (.NET + SQL Server)
- **API RESTful:** Desarrollada en **C# / ASP.NET Core** con arquitectura desacoplada en 3 proyectos (`Trotamundos`, `Negocio`, `Datos`), endpoints HTTP organizados por dominio y tareas en segundo plano (`LimpiezaReportesService`).
- **Acceso a Datos:** **Entity Framework Core** con migraciones code-first, carga inicial de países y catálogo mundial de ciudades (`ciudades-mundo.json`), e implementación del patrón **Repository**.
- **Base de Datos:** **SQL Server** para la persistencia relacional de usuarios, países, ciudades, publicaciones, alojamientos, restaurantes, experiencias gastronómicas, favoritos, notificaciones y reportes.
- **Integraciones Externas:** Servicios de geocodificación y búsqueda de lugares/agencias (Google Places / Nominatim), almacenamiento de archivos y envío de correos transaccionales.

### Frontend Móvil (React Native)
- **Core y Navegación:** **React Native** con navegación modular dividida en flujos (`AuthStack`, `MainTabNavigator`, `PublicacionStack`, `AdminStack`), hooks personalizados (`useAuth`, `usePaises`, `usePublicaciones`, `useWizard`) y Context API para el estado global de sesión y borradores del wizard.
- **Cliente HTTP:** **Axios** con interceptores para inyección automática de tokens JWT y manejo centralizado de respuestas `401`.
- **Visualización 3D y Mapas:** Integración de **Cesium** dentro de `WebView` (móvil) e `iframe` (web) utilizando imágenes Natural Earth II sin requerir tokens obligatorios (con soporte opcional para `EXPO_PUBLIC_CESIUM_ION_TOKEN`), complementado con geocodificación vía **Nominatim**.

---

## Estructura del proyecto

### Backend — Solución `Trotamundos` (Arquitectura en 3 capas)

```text
Trotamundos/                            # Solución .NET (3 proyectos)
├── Trotamundos/                        # Capa de Presentación (Web API)
│   ├── Controllers/                    # Endpoints RESTful por recurso
│   │   ├── AdminController.cs
│   │   ├── AgenciasController.cs
│   │   ├── ArchivosController.cs
│   │   ├── AuthController.cs
│   │   ├── FavoritosController.cs
│   │   ├── LugaresController.cs
│   │   ├── MapasController.cs
│   │   ├── PublicacionesController.cs
│   │   ├── ReportesController.cs
│   │   ├── SuperAdminController.cs
│   │   └── UsuariosController.cs
│   ├── Services/                       # Servicios hospedados / tareas en segundo plano
│   │   └── LimpiezaReportesService.cs
│   ├── wwwroot/                        # Archivos estáticos y recursos públicos
│   ├── appsettings.json
│   ├── Program.cs                      # Configuración de pipeline, DI, CORS y JWT
│   └── Trotamundos.http                # Colección de pruebas de endpoints HTTP
│
├── Negocio/                            # Capa de Lógica de Negocio y Dominio
│   ├── DTOs/                           # Objetos de transferencia de datos
│   │   ├── AgenciaWebDto.cs
│   │   ├── AuthDTO.cs
│   │   ├── InvitacionAdminDtos.cs
│   │   ├── LugarDto.cs
│   │   ├── MapaDTOs.cs
│   │   ├── PublicacionDTOs.cs
│   │   ├── ReporteDto.cs
│   │   └── SuperAdminDtos.cs
│   ├── Entidades/                      # Modelos del dominio
│   │   ├── AgenciaTurismo.cs
│   │   ├── Alojamiento.cs
│   │   ├── Ciudad.cs
│   │   ├── CorreoBloqueado.cs
│   │   ├── ExperienciaGastronomica.cs
│   │   ├── InvitacionAdmin.cs
│   │   ├── Notificacion.cs
│   │   ├── Pais.cs
│   │   ├── Publicacion.cs
│   │   ├── PublicacionFavorita.cs
│   │   ├── PublicacionImagen.cs
│   │   ├── Recomendacion.cs
│   │   ├── Reporte.cs
│   │   ├── Restaurante.cs
│   │   ├── TokenRecuperacion.cs
│   │   └── Usuario.cs
│   ├── Interfaces/                     # Contratos de repositorios y servicios
│   │   ├── IAuthService.cs
│   │   ├── IBusquedaAgencias.cs
│   │   ├── IBusquedaLugares.cs
│   │   ├── ICatalogoCiudades.cs
│   │   ├── ICorreoBloqueadoRepository.cs
│   │   ├── IEmailService.cs
│   │   ├── IFileStorageService.cs
│   │   ├── IInvitacionAdminRepository.cs
│   │   ├── IInvitacionAdminService.cs
│   │   ├── IMapaRepository.cs
│   │   ├── IMapaService.cs
│   │   ├── IPublicacionRepository.cs
│   │   ├── IPublicacionService.cs
│   │   ├── IReporteRepository.cs
│   │   ├── IReporteService.cs
│   │   ├── ISuperAdminService.cs
│   │   └── IUsuarioRepository.cs
│   └── Services/                       # Implementación de reglas de negocio
│       ├── AuthService.cs
│       ├── CorreoNoEnviadoException.cs
│       ├── InstagramHelper.cs
│       ├── InvitacionAdminService.cs
│       ├── MapaService.cs
│       ├── PasswordHasher.cs
│       ├── PublicacionService.cs
│       ├── ReporteService.cs
│       └── SuperAdminService.cs
│
└── Datos/                              # Capa de Acceso a Datos (EF Core + SQL Server)
    ├── Data/                           # Contexto y semillas iniciales de datos
    │   ├── ciudades-mundo.json
    │   ├── PaisesSeed.cs
    │   └── TrotamundosDbContext.cs
    ├── Migrations/                     # Historial de migraciones de Entity Framework
    ├── Repositories/                   # Implementación de repositorios por entidad
    │   ├── CorreoBloqueadoRepository.cs
    │   ├── InvitacionAdminRepository.cs
    │   ├── MapaRepository.cs
    │   ├── PublicacionRepository.cs
    │   ├── ReporteRepository.cs
    │   └── UsuarioRepository.cs
    └── Services/                       # Servicios de infraestructura e integraciones
        ├── BusquedaAgenciasService.cs
        ├── BusquedaLugaresService.cs
        ├── CatalogoCiudadesService.cs
        ├── EmailService.cs
        ├── FileStorageService.cs
        └── GoogleBusquedaLugaresService.cs
```[cite: 4, 5, 6, 7]

### Frontend — Aplicación Móvil (`src/`)

```text
src/
├── api/            # Cliente Axios (interceptores JWT + 401), módulos por recurso y geocoding (Nominatim)
├── components/     # Componentes reutilizables: common/, globe/, mapa/, publicacion/, wizard/
├── constants/      # Configuración de entorno (config.js) y paleta visual (colors.js)
├── context/        # AuthContext (gestión de sesión) y WizardContext (estado del formulario de 6 pasos)
├── hooks/          # Custom hooks: useAuth, usePaises, usePublicaciones, useWizard
├── navigation/     # Navegación modular: AppNavigator, AuthStack, MainTabNavigator, PublicacionStack, AdminStack
├── screens/        # Vistas organizadas por flujo: auth/, main/, publicaciones/, wizard/, admin/
└── utils/          # Almacenamiento seguro (SecureStore / localStorage), formateadores y diálogos
