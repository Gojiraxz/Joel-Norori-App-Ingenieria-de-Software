
$(function () {
    const config = window.estudiantesConfig || {};
    const modal = new bootstrap.Modal("#estudianteModal");
    let isEdit = false;

    // Validación del formulario
    const estudianteValidator = new JustValidate("#estudianteForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: "is-invalid",
        successFieldCssClass: "is-valid"
    });

    estudianteValidator
        .addField("#estudianteNombre", [
            { rule: "required", errorMessage: "El nombre es obligatorio" },
            { rule: "minLength", value: 2, errorMessage: "Mínimo 2 caracteres" },
            { rule: "maxLength", value: 100, errorMessage: "Máximo 100 caracteres" }
        ])
        .addField("#estudianteApellido", [
            { rule: "required", errorMessage: "El apellido es obligatorio" },
            { rule: "minLength", value: 2, errorMessage: "Mínimo 2 caracteres" },
            { rule: "maxLength", value: 100, errorMessage: "Máximo 100 caracteres" }
        ])
        .addField("#estudianteEmail", [
            { rule: "required", errorMessage: "El email es obligatorio" },
            { rule: "email", errorMessage: "Ingrese un email válido" },
            { rule: "maxLength", value: 100, errorMessage: "Máximo 100 caracteres" }
        ])
        .addField("#estudianteCursoId", [
            { rule: "required", errorMessage: "Seleccione un curso" }
        ]);

    // Configuración de DataTables
    const table = $("#estudianteTable").DataTable({
        processing: true,
        serverSide: true,

        ajax: {
            url: "/Estudiante/GetData",
            type: "POST"
        },

        columns: [
            { data: "id" },
            { data: "nombre" },
            { data: "apellido" },
            { data: "email" },
            { data: "cursoNombre" },
            {
                data: null,
                orderable: false,
                searchable: false,

                render: function (data) {
                    let buttons = "";

                    if (config.canUpdate) {
                        buttons += `
                            <button class="btn btn-sm btn-outline-primary me-1 btn-edit"
                                    data-id="${data.id}">
                                <i class="fa-solid fa-pen"></i>
                            </button>`;
                    }

                    if (config.canDelete) {
                        buttons += `
                            <button class="btn btn-sm btn-outline-danger btn-delete"
                                    data-id="${data.id}" data-nombre="${data.nombre}" data-apellido="${data.apellido}">
                                <i class="fa-solid fa-trash"></i>
                            </button>`;
                    }

                    return buttons || '<span class="text-muted">-</span>';
                }
            }
        ],

        language: {
            url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json"
        },

        order: [[1, "asc"]]
    });

    // Limpiar formulario
    function clearForm() {
        $("#estudianteId").val("");
        $("#estudianteNombre").val("");
        $("#estudianteApellido").val("");
        $("#estudianteEmail").val("");
        $("#estudianteCursoId").val("");

        estudianteValidator.refresh();
    }

    // Nuevo estudiante
    $("#btnNuevoEstudiante").on("click", function () {
        isEdit = false;
        clearForm();
        $("#estudianteModalTitle").text("Nuevo estudiante");
        modal.show();
    });
    $('#estudiantTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.get(`/Estudiante/GetById=${id}`);
            isEdit = true;
            $('#estudianteId').val(response.data.id);
            $('#estudianteNombre').val(response.data.nombre);
            $('#estudianteApellido').val(response.data.apellido);
            $('#estudianteEmail').val(response.data.email);
            $('#estudianteCursoId').val(response.data.cursoId);
            $('#estudianteModalTitle').text("editar estudiante");
            estudianteValidator.refresh();
            modal.show();
        } catch {
            Swal.fire('Error', 'No se puede cargar el estudiante', 'error');
        }

    });
    $("#estudianteTable").on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eleminar estudiante?',
            html: `¿Seguro que deseas eliminar a<strong>${nombre}</strong>`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Si, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            showCancelButtonColor: '#6c757d'
        });
        if (!confirm.isConfirmed) return
        try {
            const response = await axios.post(`/Estudiante/Delete?id=${id}`);
            if (response.data.success) {
                table.ajax.reload();
                Swal.fire('Eliminado', response.data.message, 'success');
            }
        }
        catch (error) {
            const message = error.response?.data.message || 'No se pudo eliminar';
            Swal.fire('Error', message, 'error');
        }
    });

    estudianteValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#estudianteId').val() ? parseInt($('#estudianteId').val()) : null,
            nombre: $('#estudianteNombre').val().trim(),
            apellido: $('#estudianteApellido').val().trim(),
            email: $('#estudianteEmail'). val().trim(),
            cursoId: parseInt($('#estudianteCursoId').val())
        };
        try {
            const response = isEdit
                ? await axios.post(`/Estudiante/Update`, payload)
                : await axios.post(`/Estudiante/Create`, payload);
            if (response.data.success) {
                modal.hide();
                table.ajax.reload();
                Swal.fire({
                    title: 'listo',
                    text: response.data.message,
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                });

              }

         } catch (error) { 
              const message = error.response?.data?.message || 'No se pudo guardar';
              Swal.fire('Error', message, 'error'); 
         }
    })
});
