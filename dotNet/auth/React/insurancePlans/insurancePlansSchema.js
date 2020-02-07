import * as Yup from "yup";

const insurancePlansValidate = Yup.object().shape({
  name: Yup.string()
    .min(1, "Must be at least 1 characters")
    .max(200, "Must be 200 characters or less"),
  code: Yup.string()
    .min(1, "Must be at least 1 character")
    .max(50, "Must be 50 characters or less"),
  insuranceProviderId: Yup.number()
    .min(1)
    .max(Number.MAX_SAFE_INTEGER)
    .required("Required"),
  planTypeId: Yup.number()
    .min(1)
    .max(Number.MAX_SAFE_INTEGER)
    .required("Required"),
  planStatusId: Yup.number()
    .min(1)
    .max(Number.MAX_SAFE_INTEGER)
    .required("Required"),
  minAge: Yup.number()
    .min(0)
    .max(Number.MAX_SAFE_INTEGER),
  maxAge: Yup.number()
    .min(1)
    .max(Number.MAX_SAFE_INTEGER)
});

export { insurancePlansValidate };
