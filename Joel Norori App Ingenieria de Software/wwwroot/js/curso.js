$(function () {
    const config = window.cursoConfig || {};
    const modal = new bootstrap.Modal("#cursoModal");
    let isEdit = false;

    // Validación del formulario
    const cursoValidator = new JustValidate("#cursoForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: "is-invalid",
        successFieldCssClass: "is-valid"
    });

    cursoValidator
        .addField("#cursoNombre", [
            { rule: "required", errorMessage: "El nombre es obligatorio" },
            { rule: "minLength", value: 2, errorMessage: "Mínimo 2 caracteres" },
            { rule: "maxLength", value: 100, errorMessage: "Máximo 100 caracteres" }
        ])
        .addField("#cursoDescripcion", [
            { rule: "maxLength", value: 100, errorMessage: "Máximo 100 caracteres" }
        ]);
       

    // Configuración de DataTables
    const table = $("#cursoTable").DataTable({
        processing: true,
        serverSide: true,

        ajax: {
            url: "/Curso/GetData",
            type: "POST"
        },

        columns: [
            { data: "id" },
            { data: "nombre" },
            { data: "descripcion", render: d => d || '-' },
            { data: "totalEstudiantes"},
            {
                data: null,
                orderable: false,
                searchable: false,

                render: function (data) {
                    let buttons = '';

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
        $("#cursoId").val("");
        $("#cursoNombre").val("");
        $("#cursoDescripcion").val("");
  
        cursoValidator.refresh();
    }

    // Nuevo estudiante
    $("#btnNuevoCurso").on("click", function () {
        isEdit = false;

        clearForm();

        $("#cursoModalTitle").text("Nuevo curso");
        modal.show();
    });
    $('#cursoTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.get(`/Curso/GetById?Id=${id}`);
            isEdit = true;
            $('#cursoId').val(response.data.id);
            $('#cursoNombre').val(response.data.nombre);
            $('#cursoDescripcion').val(response.data.descripcion);
            
            $('#cursoModalTitle').text("editar curso");
            cursoValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se puede cargar el curso', 'error');
        }

    });
    $("#cursoTable").on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eleminar curso?',
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
            const response = await axios.post(`/Curso/Delete?id=${id}`);
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

    cursoValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#cursoId').val() ? parseInt($('#cursoId').val()) : null,
            nombre: $('#cursoNombre').val().trim(),
            descripcion: $('#cursoDescripcion').val().trim() || null
        
        };
        try {
            const response = isEdit
                ? await axios.post(`/Curso/Update`, payload)
                : await axios.post(`/Curso/Create`, payload);
            if (response.data.success) {
                modal.hide();
                table.ajax.reload();
                Swal.fire({
                    title: 'Listo',
                    text: response.data.message,
                    icon: 'success',
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