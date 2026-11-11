$(function () {
    const config = window.permisosConfig || {};
    const modal = new bootstrap.Modal("#permisoModal");
    let isEdit = false;

    // Validación del formulario
    const permisoValidator = new JustValidate("#permisoForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: "is-invalid",
        successFieldCssClass: "is-valid"
    });

    permisoValidator
        .addField("#permisoNombre", [
            { rule: "required", errorMessage: "El nombre es obligatorio" },
            { rule: "maxLength", value: 100, errorMessage: "Máximo 100 caracteres" },
            { rule: "customRegexp", value: /^[a-z][a-z0-9._]*$/, errorMessage: "Use formato: recursos.action(eje:estudiante.crear)"}

        ])
        .addField("#permisoDescripcion", [

            { rule: "maxLength", value: 250, errorMessage: "Máximo 250 caracteres" }
        ]);


    const table = $("#permisoTable").DataTable({
        processing: true,
        serverSide: true,

        ajax: {
            url: "/Admin/GetPermissionsData",
            type: "POST"
        },

        columns: [
            { data: "id" },
            {
                data: "nombre",
                render:d=> `<code>${d}</code>`
            },
            { data: "descripcion" },
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
                                    data-id="${data.id}" data-nombre="${data.nombre}">
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
        $("#permisoId").val("");
        $("#permisoNombre").val("");
        $("#permisoDescripcion").val("");
        cursoValidator.refresh();
    }

    // Nuevo estudiante
    $("#btnNuevoPermiso").on("click", function () {
        isEdit = false;

        clearForm();

        $("#permisoModalLabel").text("Nuevo permiso");
        modal.show();
    });
    $('#permisoTable').on('click', 'btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/permiso/GetPermissionById=${id}`);
            isEdit = true;
            $('#permisoId').val(response.data.id);
            $('#permisoNombre').val(response.data.id);
            $('#permisoDescripcion').val(response.data.id);

            $('#permisoModalTitle').text("editar permiso");
            cursoValidator.refresh();

        } catch {
            Swal.fire('Error', 'No se puede cargar el permiso', 'error');
        }

    });
    $("#permisoTable").on('click', 'btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eleminar permiso?',
            html: `¿Seguro que deseas eliminar el permiso<strong>${nombre}</strong>`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Si, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            showCancelButtonColor: '#6c757d'
        });
        if (!confirm.isConfirmed) return
        try {
            const response = await axios.post(`/Admin/GetPermissionById=${id}`);
            if (response.data.success) {
                table.ajax.reload();
                Swal.fire('Eliminado', reponse.data.message, 'success');
            }
        }
        catch (error) {
            const message = error.response?.data.message || 'No se pudo eliminar';
            Swal.fire('Error', message, 'error');
        }
    });

    permisoValidator.onSucces(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#permisoId').val() ? parseInt($('#cursoId').val()) : null,
            nombre: $('#permisoNombre').val().trim(),
            descripcion: $('#permisoDescripcion').val().trim() || null

        };
        try {
            const response = isEdit
                ? await axios.post(`/Admin/UpdatePermission`, payload)
                : await axios.post(`/Admin/CreatePermission`, payload);
            if (reponse.data.success) {
                modal.hide();
                table.ajax.reload();
                Swal.fire({
                    title: 'listo',
                    text: response.data.message,
                    icon: 'succes',
                    timer: 2000,
                    showConfirmButton: false
                });

            }

        } catch (error) {
            const message = error.reponse?.data?.message || 'No se pudo guardar';
            Swal.fire('Error', message, 'error');
        }
    })
});