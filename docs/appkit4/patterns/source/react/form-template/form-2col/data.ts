
type TextLanguages = {
    [key: string]: {
        name: string;
        value: {
            title?: string;
            name?: string;
            loginText?: string;
            ssoLoginText?: string;
            userName?: string;
            password?: string;
            rememberMe?: string;
            forgotPassword?: string;
            pwcidam?: string;
            [key: string]: any;
            createAccount?: string;
            confirmPassword?: string;
            detailsTitle?: string;
            category?: string;
            subCategory?: string;
            dueDate?: string;
            detailsLabel?: string;
            cancelText?: string;
            submitText?: string;

        }
    }
}

const languages = [
    { label: 'English', value: 'en' },
    { label: 'Dansk', value: 'dnk' }
    // { label: 'Chinese', value: 'cn' },
    // { label: 'Dansk', value: 'dnk' },
    // { label: 'Deutsch', value: 'de-DE' },
    // { label: 'Svensk', value: 'sv' },
    // { label: 'Polskie', value: 'polskie' },
    // { label: 'Italiano', value: 'it' },
    // { label: 'Español', value: 'spanish' },
    // { label: 'Françaiss', value: 'french' },
];
const validationMessageLanguages = {
    passwordsNotMatch: {
        en: 'Passwords do not match',
        dnk: 'Kodeordene er ikke ens'
    },
    titleRequired: {
        en: 'Please enter a title',
        dnk: 'Indtast venligst en titel'
    },
    dateRequired: {
        en: 'Date is required',
        dnk: 'Dato er påkrævet'
    },
    descriptionRequired: {
        en: 'Description is required',
        dnk: 'Beskrivelse er påkrævet'
    },
    emailRequired: {
        en: 'Email is required',
        dnk: 'E-mail er påkrævet'
    },
    emailValid: {
        en: 'Please enter a valid email address',
        dnk: 'Indtast venligst en gyldig e-mailadresse'
    },
    validEmail: {
        en: 'Incorrect email format',
        dnk: 'Forkert e-mail-format'
    },
    validDate: {
        en: 'Please enter a valid date',
        dnk: 'Indtast venligst en gyldig dato'
    },
    firstNameRequired: {
        en: 'First name is required',
        dnk: 'Fornavn er påkrævet'
    },
    lastNameRequired: {
        en: 'Last name is required',
        dnk: 'Efternavn er påkrævet'
    },
    passwordRequired: {
        en: 'Password is required',
        dnk: 'adgangskode er påkrævet'
    },
    passwordLetter: {
        en: 'Password must contain letter',
        dnk: 'Password must contain letter'
    },
    passwordNumber: {
        en: 'Password must contain number',
        dnk: 'Adgangskode skal indeholde bogstav'
    },
    passwordCharacter: {
        en: 'Password must contain special character',
        dnk: 'Adgangskoden skal indeholde specialtegn'
    },
    
}

const textLanguages: TextLanguages = {
    en: {
        name: 'english',
        value: {
            title: 'Welcome to',
            name: 'Product Name',
            loginText: 'Login',
            ssoLoginText: 'SSO Login',
            userName: 'User name',
            password: 'Password',
            rememberMe: 'Remember me',
            forgotPassword: 'Forgot password?',
            pwcidam: 'PWC IDAM',
            firstName: 'First name',
            lastName: 'Last name',
            confirmPassword: 'Confirm password',
            createAccount: 'Create Account',
            detailsTitle: 'Title',
            category: 'Category',
            subCategory: 'Subcategory',
            dueDate: 'Due date',
            detailsLabel: 'Label',
            titleLabel: 'Title',
            cancelText: 'Cancel',
            submitText: 'Submit',
            emailValidationSuggestion: 'Did you mean',
            validLetter: 'Letter',
            validNumber: 'Number',
            validCharacter: 'Special character'
        }
    },
    dnk: {
        name: 'Dansk',
        value: {
            title: 'Velkommen til',
            name: 'produktnavn',
            loginText: 'Log på',
            ssoLoginText: 'SSO Log på',
            userName: 'Brugernavn',
            password: 'Adgangskode',
            rememberMe: 'Husk mig',
            forgotPassword: 'Glemt kodeord?',
            pwcidam: 'PWC IDAM',
            firstName: 'Fornavn',
            lastName: 'Efternavn',
            confirmPassword: 'Bekræft kodeord',
            createAccount: 'Opret konto',
            detailsTitle: 'Titel',
            category: 'Kategori',
            subCategory: 'Underkategori',
            dueDate: 'Afleveringsdato',
            detailsLabel: 'etiket',
            titleLabel: 'Titel',
            cancelText: 'Afbestille',
            submitText: 'Indsend',
            emailValidationSuggestion: 'Mente du',
            validLetter: 'Brev',
            validNumber: 'Nummer',
            validCharacter: 'Speciel karakter'
        }
    }
}

export {
    languages,
    textLanguages,
    validationMessageLanguages
}

// cn: {
//     name: 'Chinese',
//     value: {
//         title: '欢迎',
//         name: 'Product Name',
//         loginText: '登陆',
//         ssoLoginText: 'SSO 登陆',
//         userName: '用户名',
//         password: '密码',
//         rememberMe: '保持登入狀態',
//         forgotPassword: '忘记密码?',
//         pwcidam: 'PWC IDAM',
//         firstName: '姓',
//         lastName: '名',
//         confirmPassword: '确认密码',
//         createAccount: '创建',
//         detailsTitle: '标题',
//         category: '类别',
//         subCategory: '子类别',
//         dueDate: '截止日期',
//         detailsLabel: '标签',
//         cancelText: '取消',
//         submitText: '提交'
//     }
// }
