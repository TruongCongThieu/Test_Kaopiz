var LoginController = function () {
    this.initialize = function () {
        registerEvents();
    }

    var registerEvents = function () {
        $('#formLogin').validate({
            errorClass: 'text-danger',
            ignore: [],
            rules: {
                email: {
                    required: true,
                    email: true,
                    maxlength: 255,
                    normalizer: function (value) { return value.trim(); },
                    pattern: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
                },
                password: {
                    required: true,
                    minlength: 8,
                    maxlength: 100,
                    normalizer: function (value) { return value.trim(); },
                    pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:"<>?|])[A-Za-z\d!@#$%^&*()_+:"<>?|]{8,}$/
                }
            },
            messages: {
                email: {
                    required: "Vui lòng nhập email!",
                    email: "Email không hợp lệ",
                    maxlength: "Chiều dài tối đa là 255 ký tự",
                    pattern: "Email không hợp lệ"
                },
                password: {
                    required: "Vui lòng nhập mật khẩu",
                    minlength: "Chiều dài tối thiểu là 8 ký tự",
                    maxlength: "Chiều dài tối đa là 100 ký tự",
                    pattern: "Mật khẩu phải chứa ít nhất một chữ hoa, một chữ thường, một số và một ký tự đặc biệt (@$!%*?&#)"
                }
            },
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else if (element.attr("name") === "terms") {
                    error.appendTo("#terms-error-container");
                } else {
                    error.insertAfter(element);
                }
            }
        });
        $('#registrationForm').validate({
            errorClass: 'text-danger mb-0',
            ignore: [],
            rules: {
                userNameRegister: {
                    required: true,
                    maxlength: 255,
                    normalizer: function (value) { return value.trim(); },
                },
                emailRegister: {
                    required: true,
                    email: true,
                    maxlength: 255,
                    normalizer: function (value) { return value.trim(); },
                    pattern: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
                },
                passwordRegister: {
                    required: true,
                    minlength: 8,
                    maxlength: 100,
                    normalizer: function (value) { return value.trim(); },
                    pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+:"<>?|])[A-Za-z\d!@#$%^&*()_+:"<>?|]{8,}$/
                },
                confirmPasswordRegister: {
                    required: true,
                    equalTo: "#passwordRegister"
                }
            },
            messages: {
                userNameRegister: {
                    required: "Vui lòng nhập họ và tênc",
                    maxlength: "Chiều dài tối đa là 255 ký tự",
                },
                emailRegister: {
                    required: "Vui lòng nhập email",
                    email: "Email không hợp lệ",
                    maxlength: "Chiều dài tối đa là 255 ký tự",
                    pattern: "Email không hợp lệ"
                },
                passwordRegister: {
                    required: "Vui lòng nhập mật khẩu",
                    minlength: "Chiều dài tối thiểu là 8 ký tự",
                    maxlength: "Chiều dài tối đa là 20 ký tự",
                    pattern: "Mật khẩu phải chứa ít nhất một chữ hoa, một chữ thường, một số và một ký tự đặc biệt (@$!%*?&#)"
                },
                confirmPasswordRegister: {
                    required: "Vui lòng nhập xác nhận mật khẩu",
                    equalTo: "Xác nhận mật khẩu không khớp"
                },
            },
            errorPlacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else if (element.attr("name") === "terms") {
                    error.appendTo("#terms-error-container");
                } else {
                    error.insertAfter(element);
                }
            }
        });

        // Toggle show/hide for any eye icon inside an input-group
        $(document).on('click', '.togglePassword', function () {
            const $icon = $(this); 
            const $input = $icon.closest('.input-group').find('input');
            const isPassword = $input.attr('type') === 'password';
            $input.attr('type', isPassword ? 'text' : 'password');
            $icon.toggleClass('fa-eye fa-eye-slash');
        });

        $('#btnLogin').on('click', function (e) {
            if ($('#formLogin').valid()) {
                e.preventDefault();
                $('#btnLogin').prop('disabled', true);
                var email = $('#email').val();
                var password = $('#password').val();
                var rememberMe = $('#rememberMe').is(':checked');
                login(email, password, rememberMe);
            }
        });

        $('#registrationForm').on('submit', function (e) {
            e.preventDefault();
            if (!$('#registrationForm').valid()) return;
            var userName = $('#userNameRegister').val();
            var email = $('#emailRegister').val();
            var password = $('#passwordRegister').val();
            $.ajax({
                type: 'POST',
                url: '/dang-ky-ngay',
                contentType: 'application/json',
                data: JSON.stringify({
                    userName: userName,
                    email: email,
                    password: password,
                }),
                dataType: 'json',
                success: function (res) {
                    if (res.succeeded) {
                        base.notify('Đăng ký thành công', 'success')
                        setTimeout(function () {
                            window.location.href = '/dang-nhap';
                        }, 2000);
                       
                    } else {
                        base.notify(res.messages, 'error');
                    }
                },
                error: function () { base.notify('Đang xảy ra lỗi', 'error'); }
            });
        });
    }

    var login = function (email, password, rememberMe) {
        $.ajax({
            type: 'POST',
            url: '/login-post' + window.location.search,
            contentType: "application/json",
            data: JSON.stringify({
                email: email,
                password: password,
                rememberMe: rememberMe
            }),
            dataType: 'json',
            success: function (res) {
                if (res.succeeded) {
                   window.location.href = "/trang-chu";
                }
                else {
                    base.notify(res.messages, 'error');
                }
            },
            error: function () {
                base.notify('Đang xảy ra lỗi', 'error');
            },
            complete: function () {
                $('#btnLogin').prop('disabled', false);
                base.stopLoading();
            }
        })
    }
}